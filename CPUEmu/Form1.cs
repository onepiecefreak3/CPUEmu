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
using CPUEmu.Contract;

namespace CPUEmu
{
    public partial class Form1 : Form
    {
        private Emulator _emu;
        private string _currentFile;

        private bool _abort;
        private bool _stopped;
        private bool _doStep;
        private bool _updating;
        private bool _printToggle = true;

        System.Timers.Timer _refreshTimer;
        System.Timers.Timer _disassembleTimer;

        public Form1()
        {
            InitializeComponent();

            _refreshTimer = new System.Timers.Timer(16.666);
            _refreshTimer.Elapsed += RefreshTables;

            _disassembleTimer = new System.Timers.Timer(40);
            _disassembleTimer.Elapsed += RefreshDisassembly;
        }

        #region Asynchronous tasks
        private void RefreshTables(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_updating)
            {
                RefreshFlags();
                RefreshRegisters();
            }
        }

        private Func<long, string, string> CreateDisassemblyLine => new Func<long, string, string>((l, s) => $"{l:X4}: {s}" + Environment.NewLine);
        private void RefreshDisassembly(object sender, System.Timers.ElapsedEventArgs e)
        {
            var list = _emu.DisassembleInstructions(_emu.PayloadOffset, 20).ToList();

            MultipleThreadControlExec(txtDisassembly, new Action<Control>((c) =>
            {
                var str = list.Aggregate("", (a, b) => a + CreateDisassemblyLine(b.offset, b.source));
                c.Text = str;
            }));

            if (_stopped)
            {
                //TODO: Only when stopped, mark current instruction line
                var currentLineIndex = list.FindIndex(x => x.offset == _emu.CurrentInstructionOffset);
                var selectionStart = list.TakeWhile((x, i) => i < currentLineIndex).Sum(x => CreateDisassemblyLine(x.offset, x.source).Length) - currentLineIndex;

                if (currentLineIndex >= 0)
                    MultipleThreadControlExec(txtDisassembly, new Action<Control>((c) =>
                    {
                        (c as TextBoxBase).Select(selectionStart, CreateDisassemblyLine(list[currentLineIndex].offset, list[currentLineIndex].source).Length - 1);
                        (c as RichTextBox).SelectionBackColor = Color.Yellow;
                    }));
            }
        }

        private async void StartExecution()
        {
            await ExecuteEmulator();

            FinishExecution();
        }
        #endregion

        #region Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofd.FileName);
            }
        }
        #endregion

        private void OpenFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);
            _currentFile = file;

            OpenEmulator(file);
            ResetUI();

            StartExecution();

            _updating = true;

            _refreshTimer.Start();
            _disassembleTimer.Start();
        }

        private void ResetUI()
        {
            txtlog.Clear();
            txtDisassembly.Clear();

            _abort = false;
            _stopped = false;
            _doStep = false;

            btnStop.Text = "Stop Execution";

            btnAbort.Enabled = true;
            btnStop.Enabled = true;
            btnStep.Enabled = false;
            btnReExec.Enabled = false;
        }

        private void OpenEmulator(string file)
        {
            //TODO: Use MEF for loading emulators from plugins
            _emu = new AARCH32(File.ReadAllBytes(file), 0x100000, 0x1000000, 0x100000);
        }

        private Task ExecuteEmulator()
        {
            return Task.Factory.StartNew(() =>
            {
                while (!_emu.IsFinished && !_abort)
                {
                    if (!_stopped || _doStep)
                    {
                        if (_doStep)
                            _doStep = false;

                        _emu.ExecuteNextInstruction();
                    }

                    Thread.Sleep(200);
                }
            });
        }

        private void FinishExecution()
        {
            btnAbort.Enabled = false;
            btnStop.Enabled = false;
            btnStep.Enabled = false;
            btnReExec.Enabled = true;

            _abort = false;
            _stopped = false;
            _doStep = false;
            _updating = false;

            _refreshTimer.Stop();
            _disassembleTimer.Stop();

            Log("Finished.");
        }

        private void MultipleThreadControlExec(Control control, Action<Control> action)
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => action(control)));
            else
                action(control);
        }

        private void RefreshTable(IEnumerable<(string name, long value)> table, TextBoxBase box, Func<string, string, long, string> addEntry)
        {
            var str = "";
            foreach (var entry in table)
                str = addEntry(str, entry.name, entry.value);

            MultipleThreadControlExec(box, new Action<Control>((c) => (c as TextBoxBase).Text = str));
        }

        private void RefreshFlags()
        {
            RefreshTable(_emu.RetrieveFlags(), txtFlags, new Func<string, string, long, string>((s1, s2, l) => s1 + $"{s2}: 0x{l:X8}" + Environment.NewLine));
        }

        private void RefreshRegisters()
        {
            RefreshTable(_emu.RetrieveRegisters(), txtRegs, new Func<string, string, long, string>((s1, s2, l) => s1 + $"{s2}: 0x{l:X8}" + Environment.NewLine));
        }

        #region Logging
        private void Log(string message)
        {
            MultipleThreadControlExec(txtlog, new Action<Control>((c) => (c as TextBoxBase).AppendText(message)));
        }

        private void OnEmulatorLog(object sender, string message) => Log(message + Environment.NewLine);
        #endregion

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _abort = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _stopped = btnStep.Enabled = !_stopped;

            if (_stopped)
                btnStop.Text = "Resume Execution";
            else
                btnStop.Text = "Stop Execution";
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            _doStep = true;
        }

        private void btnPrintToggle_Click(object sender, EventArgs e)
        {
            if (_printToggle)
            {
                btnPrintToggle.Text = "Enable Printing";
                if (_emu != null)
                {
                    _emu.Log -= OnEmulatorLog;
                }
            }
            else
            {
                btnPrintToggle.Text = "Disable Printing";
                if (_emu != null)
                {
                    _emu.Log += OnEmulatorLog;
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
