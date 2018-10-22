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
                if (!File.Exists(ofd.FileName))
                    throw new FileNotFoundException(ofd.FileName);

                OpenEmulator(ofd.FileName);
                consoleLog.Clear();
                StartExecution();
            }
        }

        private void OpenEmulator(string file)
        {
            //TODO: Use MEF for loading emulators from plugins
            _emu = new AARCH32(File.OpenRead(file));

            _emu.Log += OnEmulatorLog;
            _emu.Print += OnEmulatorPrint;
        }

        private void StartExecution()
        {
            _execution = Task.Factory.StartNew(() =>
             {
                 while (!_emu.IsFinished && !_abort)
                 {
                     if (!_stopped || _doStep)
                     {
                         if (_doStep)
                             _doStep = false;

                         _emu.PrintCurrentInstruction();
                         _emu.ExecuteNextInstruction();

                         UpdateFlags();
                         UpdateRegisters();
                     }

                     //Thread.Sleep(1000);
                 }

                 Log("Finished.");
             });
        }

        private void UpdateFlags()
        {
            var flags = _emu.RetrieveFlags();

            if (txtFlags.InvokeRequired)
                txtFlags.Invoke(new Action(() => txtFlags.Clear()));
            else
                txtFlags.Clear();

            foreach (var entry in flags)
                if (txtFlags.InvokeRequired)
                    txtFlags.Invoke(new Action(() => txtFlags.Text += $"{entry.Key}: 0x{entry.Value:X8}" + Environment.NewLine));
                else
                    txtFlags.Text += $"{entry.Key}: 0x{entry.Value:X8}" + Environment.NewLine;
        }

        private void UpdateRegisters()
        {
            var regs = _emu.RetrieveRegisters();

            if (txtRegs.InvokeRequired)
                txtRegs.Invoke(new Action(() => txtRegs.Clear()));
            else
                txtRegs.Clear();

            foreach (var entry in regs)
                if (txtRegs.InvokeRequired)
                    txtRegs.Invoke(new Action(() => txtRegs.Text += $"{entry.Key}: 0x{entry.Value:X8}" + Environment.NewLine));
                else
                    txtRegs.Text += $"{entry.Key}: 0x{entry.Value:X8}" + Environment.NewLine;
        }

        #region Logging
        private void Log(string message)
        {
            if (consoleLog.InvokeRequired)
                consoleLog.Invoke(new Action(() => consoleLog.AppendText(message)));
            else
                consoleLog.AppendText(message);
        }

        private void OnEmulatorLog(object sender, string message) => Log(message + Environment.NewLine);
        #endregion

        #region Disassembling
        private void Print(string message)
        {
            if (txtPrint.InvokeRequired)
                txtPrint.Invoke(new Action(() => txtPrint.AppendText(message)));
            else
                txtPrint.AppendText(message);
        }

        private void OnEmulatorPrint(object sender, string message) => Print(message + Environment.NewLine);
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
    }
}
