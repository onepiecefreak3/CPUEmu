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
        private bool _updating;

        System.Timers.Timer _timer;

        public Form1()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(16.666);
            _timer.Elapsed += RefreshUI;
        }

        private void RefreshUI(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_updating)
            {
                RefreshFlags();
                RefreshRegisters();
            }
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

            _updating = true;
            _timer.Start();
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
            _emu = new AARCH32(File.ReadAllBytes(file), 0x100000, 0x1000000, 0x100000);

            if (_printToggle)
            {
                _emu.Log += OnEmulatorLog;
                _emu.Print += OnEmulatorPrint;
            }
        }

        private async void StartExecution()
        {
            var res = await ExecuteEmulator();

            if (_printToggle) Log("Finished.");
            FinishExecution();
        }

        private Task<bool> ExecuteEmulator()
        {
            return Task.Factory.StartNew(() =>
            {
                while (!_emu.IsFinished && !_abort)
                {
                    if (!_stopped || _doStep)
                    {
                        if (_doStep)
                            _doStep = false;

                        _emu.PrintCurrentInstruction();
                        _emu.ExecuteNextInstruction();
                    }

                    Thread.Sleep(200);
                }

                return true;
            });
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
            _updating = false;
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

        private void RefreshTable(Dictionary<string, long> table, TextBoxBase box, Func<string, string, long, string> addEntry)
        {
            //MultipleThreadControlExec(box, new Action<Control>((c) => (c as TextBoxBase).Clear()));

            var str = "";
            foreach (var entry in table)
                str=addEntry(str, entry.Key, entry.Value);

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
