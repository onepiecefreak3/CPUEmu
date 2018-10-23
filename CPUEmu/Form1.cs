using System;
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

//TODO: using R15/PC as a destination register in all instructions needs to trigger SetPC or similar somehow

namespace CPUEmu
{
    public partial class Form1 : Form
    {
        private Emulator _emu;
        private string _currentFile;
        private Task _execution;
        private bool _abort;
        private bool _stopped;
        private bool _doStep;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);
            _currentFile = file;

            OpenEmulator(file);
            ResetUI();
            StartExecution();
        }

        private void ResetUI()
        {
            consoleLog.Clear();
            txtPrint.Clear();

            _stopped = false;
            btnStop.Text = "Stop Execution";

            _doStep = false;
            btnStep.Enabled = false;

            _abort = false;
            btnAbort.Enabled = true;
            btnStop.Enabled = true;

            ToggleReExecBtn(false);
        }

        private void OpenEmulator(string file)
        {
            //TODO: Use MEF for loading emulators from plugins
            _emu = new AARCH32(File.OpenRead(file));

            if (_printToggle)
            {
                _emu.Log += OnEmulatorLog;
                _emu.Print += OnEmulatorPrint;
            }
        }

        private void StartExecution()
        {
            _execution = Task.Factory.StartNew(() => ExecuteEmulator());
        }

        private void ExecuteEmulator()
        {
            while (!_emu.IsFinished && !_abort)
            {
                if (!_stopped || _doStep)
                {
                    if (_doStep)
                        _doStep = false;

                    _emu.PrintCurrentInstruction();
                    _emu.ExecuteNextInstruction();

                    RefreshFlags();
                    RefreshRegisters();
                }

                Thread.Sleep(1000);
            }

            if (_printToggle) Log("Finished.");
            FinishExecution();
        }

        private void FinishExecution()
        {
            MultipleThreadControlExec(btnReExec, new Action<Control>((c) => c.Enabled = true));
            MultipleThreadControlExec(btnStop, new Action<Control>((c) => c.Enabled = false));
            MultipleThreadControlExec(btnStep, new Action<Control>((c) => c.Enabled = false));
            MultipleThreadControlExec(btnAbort, new Action<Control>((c) => c.Enabled = false));

            _abort = false;
            _doStep = false;
            _stopped = false;
        }

        private void MultipleThreadControlExec(Control control, Action<Control> action)
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => action(control)));
            else
                action(control);
        }

        private void ToggleReExecBtn(bool state)
        {
            MultipleThreadControlExec(btnReExec, new Action<Control>((c) => c.Enabled = state));
        }

        private void RefreshTable(Dictionary<string, long> table, TextBoxBase box, Action<TextBoxBase, string, long> addEntry)
        {
            MultipleThreadControlExec(box, new Action<Control>((c) => (c as TextBoxBase).Clear()));

            foreach (var entry in table)
                MultipleThreadControlExec(box, new Action<Control>((c) => addEntry(c as TextBoxBase, entry.Key, entry.Value)));
        }

        private void RefreshFlags()
        {
            RefreshTable(_emu.RetrieveFlags(), txtFlags, new Action<TextBoxBase, string, long>((box, s, l) => box.Text += $"{s}: 0x{l:X8}" + Environment.NewLine));
        }

        private void RefreshRegisters()
        {
            RefreshTable(_emu.RetrieveRegisters(), txtRegs, new Action<TextBoxBase, string, long>((box, s, l) => box.Text += $"{s}: 0x{l:X8}" + Environment.NewLine));
        }

        #region Logging
        private void Log(string message)
        {
            MultipleThreadControlExec(consoleLog, new Action<Control>((c) => (c as TextBoxBase).AppendText(message)));
        }

        private void OnEmulatorLog(object sender, string message) => Log(message + Environment.NewLine);
        #endregion

        #region Disassembling
        private void Print(string message)
        {
            MultipleThreadControlExec(txtPrint, new Action<Control>((c) => (c as TextBoxBase).AppendText(message)));
        }

        private void OnEmulatorPrint(object sender, string message) => Print(message);
        #endregion

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _abort = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _stopped = !_stopped;
            btnStep.Enabled = _stopped;
            if (_stopped)
                btnStop.Text = "Continue Execution";
            else
                btnStop.Text = "Stop Execution";
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            _doStep = true;
        }

        private bool _printToggle = true;
        private void btnPrintToggle_Click(object sender, EventArgs e)
        {
            if (_printToggle)
            {
                btnPrintToggle.Text = "Enable Printing";
                if (_emu != null)
                {
                    _emu.Log -= OnEmulatorLog;
                    _emu.Print -= OnEmulatorPrint;
                }
            }
            else
            {
                btnPrintToggle.Text = "Disable Printing";
                if (_emu != null)
                {
                    _emu.Log += OnEmulatorLog;
                    _emu.Print += OnEmulatorPrint;
                }
            }
            _printToggle = !_printToggle;
        }

        private void btnReExec_Click(object sender, EventArgs e)
        {
            OpenFile(_currentFile);
        }
    }
}
