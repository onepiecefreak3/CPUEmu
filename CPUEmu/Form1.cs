﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using CPUEmu.Contract;

namespace CPUEmu
{
    public partial class Form1 : Form
    {
        private static double TwipsToPixel(int twips) => (int)(twips / 14.9999999999);

        private int _lineSpacingInTwips = 210;
        private int _timeOut = 40;

        private int _bufferDisassemblyLines;
        private int _breakPointSize;

        private Emulator _emu;
        private string _currentFile;

        private bool _abort;
        private bool _stopped;
        private bool _doStep;
        private bool _instructionRunning;

        private bool _printing;

        System.Timers.Timer _refreshTimer;
        System.Timers.Timer _disassembleTimer;

        public Form1()
        {
            InitializeComponent();

            txtDisassembly.SetInnerMargins((int)TwipsToPixel(_lineSpacingInTwips), 0, 0, 0);
            _bufferDisassemblyLines = (int)(txtDisassembly.Height / TwipsToPixel(_lineSpacingInTwips));
            _breakPointSize = (int)TwipsToPixel(_lineSpacingInTwips);

            _refreshTimer = new System.Timers.Timer(16.666);
            _refreshTimer.Elapsed += OnRefreshTables;

            _disassembleTimer = new System.Timers.Timer(40);
            _disassembleTimer.Elapsed += OnRefreshDisassembly;

            EnablePrinting();
        }

        #region Asynchronous tasks
        private void OnRefreshTables(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshFlags(true);
            RefreshRegisters(true);
        }

        private void OnRefreshDisassembly(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshDisassembly(true);
        }
        #endregion

        #region Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName))
                    throw new FileNotFoundException(ofd.FileName);
                _currentFile = ofd.FileName;

                Initialize();
                StartExecution();
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            AbortExecution();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleStopExecution();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            DoStep();
        }

        private void btnReExec_Click(object sender, EventArgs e)
        {
            Initialize();
            StartExecution();
        }

        private void btnPrintToggle_Click(object sender, EventArgs e)
        {
            TogglePrinting(!_printing);
        }

        private void txtDisassembly_SizeChanged(object sender, EventArgs e)
        {
            RecalculateDisassemblyLines();
            RefreshDisassembly(false);
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            InitializeUI();
            InitializeEmulator();
        }

        private void InitializeUI()
        {
            txtlog.Clear();
            txtDisassembly.Clear();

            _emu = null;

            _abort = false;
            _instructionRunning = false;
            _stopped = false;
            _doStep = false;

            btnAbort.Enabled = true;
            btnStop.Enabled = true;
            btnStep.Enabled = false;
            btnReExec.Enabled = false;

            btnStop.Text = "Stop Execution";
        }

        private void InitializeEmulator()
        {
            //TODO: MEF for emulator choosing
            _emu = new AARCH32(File.ReadAllBytes(_currentFile), 0x100000, 0x1000000, 0x100000);
        }

        private void StartExecution()
        {
            ExecuteEmulator();

            _refreshTimer.Start();
            _disassembleTimer.Start();
        }

        private async void ExecuteEmulator()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!_emu.IsFinished && !_abort)
                {
                    if (!_stopped || _doStep)
                    {
                        _instructionRunning = true;
                        if (_doStep) _doStep = false;

                        Thread.Sleep(_timeOut);

                        _emu?.ExecuteNextInstruction();

                        _instructionRunning = false;
                    }
                }
            });

            FinishExecution();
        }

        private void AbortExecution()
        {
            _abort = true;
        }

        private void FinishExecution()
        {
            _refreshTimer.Stop();
            _disassembleTimer.Stop();

            RefreshFlags(true);
            RefreshRegisters(true);
            RefreshDisassembly(true);

            btnAbort.Enabled = false;
            btnStop.Enabled = false;
            btnStep.Enabled = false;
            btnReExec.Enabled = true;

            _stopped = false;
            _doStep = false;

            if (_abort)
                Log("Aborted.");
            else
                Log("Finished.");

            _abort = false;
        }

        private void ToggleStopExecution()
        {
            if (_stopped)
                ResumeExecution();
            else
                StopExecution();
        }

        private void StopExecution()
        {
            _refreshTimer.Stop();
            _disassembleTimer.Stop();

            _stopped = true;
            _doStep = false;

            while (_instructionRunning)
                ;

            btnStep.Enabled = true;
            btnStop.Text = "Resume Execution";

            RefreshDisassembly(false);
        }

        private void ResumeExecution()
        {
            _stopped = false;
            _doStep = false;

            while (_instructionRunning)
                ;

            btnStep.Enabled = false;
            btnStop.Text = "Stop Execution";

            _refreshTimer.Start();
            _disassembleTimer.Start();
        }

        private void DoStep()
        {
            _doStep = true;

            while (_doStep || _instructionRunning)
                ;

            RefreshFlags(false);
            RefreshRegisters(false);

            RefreshDisassembly(false);
        }

        private void TogglePrinting(bool state)
        {
            if (state)
                EnablePrinting();
            else
                DisablePrinting();
        }

        private void DisablePrinting()
        {
            _printing = false;

            btnPrintToggle.Text = "Enable Printing";
        }

        private void EnablePrinting()
        {
            _printing = true;

            btnPrintToggle.Text = "Disable Printing";
        }

        private void RefreshTable(IEnumerable<(string name, long value)> table, TextBoxBase box, Func<string, string, long, string> addEntry, bool foreignThread)
        {
            var str = table.Aggregate("", (a, b) => a = addEntry(a, b.name, b.value));

            if (foreignThread)
                MultipleThreadControlExec(box, new Action<Control>((c) => (c as TextBoxBase).Text = str));
            else
                box.Text = str;
        }

        private void RefreshFlags(bool foreignThread)
        {
            if (_emu != null)
                RefreshTable(
                    _emu.RetrieveFlags(),
                    txtFlags,
                    new Func<string, string, long, string>((s1, s2, l) => s1 + $"{s2}: 0x{l:X8}" + Environment.NewLine),
                    foreignThread);
        }

        private void RefreshRegisters(bool foreignThread)
        {
            if (_emu != null)
                RefreshTable(
                    _emu.RetrieveRegisters(),
                    txtRegs,
                    new Func<string, string, long, string>((s1, s2, l) => s1 + $"{s2}: 0x{l:X8}" + Environment.NewLine),
                    foreignThread);
        }

        private void HighlightCurrentInstruction(bool foreignThread)
        {
            var list = GetDisassembledInstructions().ToList();

            var currentLineIndex = list.FindIndex(x => x.offset == _emu.CurrentInstructionOffset);
            var selectionStart = list.TakeWhile((x, i) => i < currentLineIndex).Sum(x => CreateDisassemblyLine(x.offset, x.source).Length) - currentLineIndex;

            if (currentLineIndex >= 0)
            {
                if (foreignThread)
                {
                    MultipleThreadControlExec(txtDisassembly, new Action<Control>((c) =>
                    {
                        (c as RichTextBox).Select(selectionStart, CreateDisassemblyLine(list[currentLineIndex].offset, list[currentLineIndex].source).Length - 1);
                        (c as RichTextBox).SelectionBackColor = Color.Yellow;
                    }));
                }
                else
                {
                    txtDisassembly.Select(selectionStart, CreateDisassemblyLine(list[currentLineIndex].offset, list[currentLineIndex].source).Length - 1);
                    txtDisassembly.SelectionBackColor = Color.Yellow;
                }
            }
        }

        private void MultipleThreadControlExec(Control control, Action<Control> action)
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => action(control)));
            else
                action(control);
        }

        private void RecalculateDisassemblyLines()
        {
            _bufferDisassemblyLines = (int)(txtDisassembly.Height / TwipsToPixel(_lineSpacingInTwips));
        }
        #endregion

        #region Disassembly
        private static Func<long, string, string> CreateDisassemblyLine => new Func<long, string, string>((l, s) => $"{l:X4}: {s}" + Environment.NewLine);

        //TODO: Find sane settings for continous disassembling
        private IEnumerable<(long offset, string source)> GetDisassembledInstructions()
            => _emu.DisassembleInstructions(
                Math.Min(
                    Math.Max(
                        _emu.PayloadOffset,
                        _emu.CurrentInstructionOffset - (_emu.BitsPerInstruction / 8) * (_bufferDisassemblyLines / 2)),
                    _emu.PayloadOffset + _emu.PayloadLength - (_emu.BitsPerInstruction / 8) * _bufferDisassemblyLines),
                _bufferDisassemblyLines);

        private void RefreshDisassembly(bool foreignThread)
        {
            var list = GetDisassembledInstructions().ToList();
            var str = list.Aggregate("", (a, b) => a + CreateDisassemblyLine(b.offset, b.source)).TrimEnd(Environment.NewLine.ToArray());

            if (foreignThread)
                MultipleThreadControlExec(txtDisassembly, new Action<Control>((c) => { c.Text = str; HighlightCurrentInstruction(true); txtDisassembly.SetLineSpacing(_lineSpacingInTwips); }));
            else
            {
                txtDisassembly.Text = str;
                HighlightCurrentInstruction(false);
                txtDisassembly.SetLineSpacing(_lineSpacingInTwips);
            }
        }
        #endregion

        #region Logging
        private void Log(string message)
        {
            if (_printing)
                MultipleThreadControlExec(txtlog, new Action<Control>((c) => (c as TextBoxBase).AppendText(message)));
        }

        private void OnEmulatorLog(object sender, string message) => Log(message + Environment.NewLine);
        #endregion
    }
}
