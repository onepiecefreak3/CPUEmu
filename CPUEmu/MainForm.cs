﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using CPUEmu.Defaults;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    public partial class MainForm : Form
    {
        private readonly PluginLoader _pluginLoader = PluginLoader.Instance;

        private IAssemblyAdapter _adapter;
        private string _currentFileName;
        private Stream _currentFileStream;

        private const int LineSpacingInTwips_ = 210;

        private static double TwipsToPixel(int twips) => (int)(twips / 14.9999999999);

        private int _breakPointSize => Math.Max(1, (int)TwipsToPixel(LineSpacingInTwips_) - 3);
        private Color _breakPointColor = Color.Red;
        private Color _currentInstructionColor = Color.Yellow;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Methods

        private void StartExecution()
        {
            _adapter.Executor.ExecuteAsync();
        }

        private void ResumeExecution()
        {
            _adapter.Executor.ResumeExecution();
        }

        private void HaltExecution()
        {
            _adapter.Executor.HaltExecution();
        }

        private void AbortExecution()
        {
            _adapter.Executor.AbortExecution();
        }

        private void OpenFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName))
                    throw new FileNotFoundException(ofd.FileName);

                _currentFileName = ofd.FileName;
                _currentFileStream = File.OpenRead(_currentFileName);

                SetupUiOpenFile();
                SelectAdapter();
            }
        }

        private void CloseFile()
        {
            _currentFileStream.Close();
            _currentFileName = null;
            _adapter.Dispose();
            _adapter = null;

            ResetUi();
        }

        private void SelectAdapter()
        {
            _adapter = _pluginLoader.Adapters.FirstOrDefault(x =>
            {
                var startPosition = _currentFileStream.Position;
                var res = x.Identify(_currentFileStream);
                _currentFileStream.Position = startPosition;
                return res;
            });
            _adapter = _pluginLoader.Adapters.First();
        }

        private void LoadAdapter()
        {
            _adapter.Load(_currentFileStream);
            SetupExecutorEvents();
        }

        private void SetupExecutorEvents()
        {
            _adapter.Executor.ExecutionStarted += Executor_ExecutionStarted;
            _adapter.Executor.ExecutionFinished += Executor_ExecutionFinished;

            _adapter.Executor.InstructionExecuting += Executor_InstructionExecuting;
            _adapter.Executor.InstructionExecuted += Executor_InstructionExecuted;

            _adapter.Executor.ExecutionHalted += Executor_ExecutionHalted;
            _adapter.Executor.ExecutionAborted += Executor_ExecutionAborted;

            _adapter.Executor.BreakpointReached += Executor_BreakpointReached;
        }

        private void WriteLogLine(string message)
        {
            if (txtlog.InvokeRequired)
                txtlog.Invoke(new MethodInvoker(() => WriteLogLine(message)));
            else
                txtlog.AppendText(message + Environment.NewLine);
        }

        private void SetBreakpoint(int absoluteIndex)
        {
            if (absoluteIndex >= _adapter.Instructions.Count)
                return;

            _adapter.Executor.SetBreakpoint(_adapter.Instructions[absoluteIndex]);
            DrawBreakpoints();
        }

        private void DrawBreakpoints()
        {
            var gr = pnBreakPoints.CreateGraphics();
            gr.Clear(pnBreakPoints.BackColor);

            var activeBreakpoints = _adapter.Executor.GetActiveBreakpoints().ToArray();
            for (int i = 0; i < Math.Min(_adapter.Instructions.Count, txtDisassembly.Height / txtDisassembly.ItemHeight); i++)
            {
                if (activeBreakpoints.Contains(txtDisassembly.Items[i + txtDisassembly.TopIndex]))
                {
                    gr.FillEllipse(new SolidBrush(_breakPointColor),
                        new Rectangle(0, i * (int)TwipsToPixel(LineSpacingInTwips_) + 5, _breakPointSize, _breakPointSize));
                }
            }

            gr.Dispose();
        }

        private void LoadDisassembly()
        {
            txtDisassembly.Items.AddRange(_adapter.Instructions.ToArray());
        }

        private void LoadFlagsAndRegisters()
        {
            if (txtFlags.InvokeRequired)
                txtFlags.Invoke(new MethodInvoker(LoadFlagsAndRegisters));
            else
            {
                txtFlags.Text = string.Join(Environment.NewLine, _adapter.Environment.CpuState.GetFlags());
                txtRegs.Text = string.Join(Environment.NewLine, _adapter.Environment.CpuState.GetRegisters());
            }
        }

        #endregion

        #region UI

        private void ResetUi()
        {
            txtlog.Clear();
            txtFlags.Clear();
            txtRegs.Clear();
            txtDisassembly.Items.Clear();

            _adapter.Executor.ResetBreakpoints();
            DrawBreakpoints();
        }

        private void SetupUiOpenFile()
        {
            txtlog.Clear();

            btnStartExecution.Enabled = true;
            btnStop.Enabled = false;
            btnAbort.Enabled = false;
            btnResume.Enabled = false;
        }

        private void SetupUiExecutionStart()
        {
            if (btnStartExecution.InvokeRequired)
                btnStartExecution.Invoke(new MethodInvoker(SetupUiExecutionStart));
            else
            {
                btnStartExecution.Enabled = false;
                btnStop.Enabled = true;
                btnResume.Enabled = false;
                btnAbort.Enabled = true;
            }
        }

        private void SetupUiExecutionFinished()
        {
            if (btnStartExecution.InvokeRequired)
                btnStartExecution.Invoke(new MethodInvoker(SetupUiExecutionFinished));
            else
            {
                btnStartExecution.Enabled = true;
                btnStop.Enabled = false;
                btnResume.Enabled = false;
                btnAbort.Enabled = false;
            }
        }

        private void SetupUiExecutionHalted()
        {
            if (btnStop.InvokeRequired)
                btnStop.Invoke(new MethodInvoker(SetupUiExecutionHalted));
            else
            {
                btnStop.Enabled = false;
                btnResume.Enabled = true;
                btnAbort.Enabled = true;
            }
        }

        private void SetupUiExecutionAborted()
        {
            if (btnStartExecution.InvokeRequired)
                btnStartExecution.Invoke(new MethodInvoker(SetupUiExecutionHalted));
            else
            {
                btnStartExecution.Enabled = true;
                btnStop.Enabled = false;
                btnResume.Enabled = false;
                btnAbort.Enabled = false;
            }
        }

        #endregion

        #region Events

        private void Executor_ExecutionHalted(object sender, EventArgs e)
        {
            SetupUiExecutionHalted();
            LoadFlagsAndRegisters();
            WriteLogLine("Execution halted.");
        }

        private void Executor_ExecutionAborted(object sender, EventArgs e)
        {
            SetupUiExecutionAborted();
            LoadFlagsAndRegisters();
            WriteLogLine("Execution aborted.");
        }

        private void Executor_InstructionExecuted(object sender, EventArgs e)
        {
        }

        private void Executor_InstructionExecuting(object sender, EventArgs e)
        {
        }

        private void Executor_ExecutionStarted(object sender, EventArgs e)
        {
            SetupUiExecutionStart();
            LoadFlagsAndRegisters();
            WriteLogLine("Execution started.");
        }

        private void Executor_ExecutionFinished(object sender, EventArgs e)
        {
            SetupUiExecutionFinished();
            LoadFlagsAndRegisters();
            WriteLogLine("Execution finished normally.");
        }

        private void Executor_BreakpointReached(object sender, EventArgs e)
        {
            SetupUiExecutionHalted();
            LoadFlagsAndRegisters();
            WriteLogLine("Breakpoint reached.");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
            if (_currentFileStream != null)
            {
                LoadAdapter();
                LoadDisassembly();
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void BtnStartExecution_Click(object sender, EventArgs e)
        {
            // TODO: Choose interrupter in GUI
            _adapter.Environment.InterruptBroker = new DefaultInterruptBroker(new DefaultLogger(txtlog));
            StartExecution();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            HaltExecution();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            AbortExecution();
        }

        private void BtnResume_Click(object sender, EventArgs e)
        {
            ResumeExecution();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _adapter?.Dispose();
        }

        private void pnBreakPoints_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentFileStream == null)
                return;

            var currentLine = (int)Math.Floor(e.Y / TwipsToPixel(LineSpacingInTwips_));
            var absoluteLine = txtDisassembly.TopIndex + currentLine;
            SetBreakpoint(absoluteLine);
        }

        private void TxtDisassembly_TopIndexChanged(object sender, EventArgs e)
        {
            DrawBreakpoints();
        }

        private void TxtDisassembly_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var absoluteLine = Math.Min(_adapter.Instructions.Count - 1, e.Index);
            var g = e.Graphics;

            // Background
            SolidBrush backgroundBrush;
            if (_adapter.Executor.IsHalted && _adapter.Executor.CurrentInstruction == _adapter.Instructions[absoluteLine])
                backgroundBrush = new SolidBrush(_currentInstructionColor);
            else if (_adapter.Executor.GetActiveBreakpoints().Contains(_adapter.Instructions[absoluteLine]))
                backgroundBrush = new SolidBrush(_breakPointColor);
            else
                backgroundBrush = new SolidBrush(txtDisassembly.BackColor);
            g.FillRectangle(backgroundBrush, e.Bounds);

            // Text
            g.DrawString(txtDisassembly.Items[e.Index].ToString(), e.Font, new SolidBrush(txtDisassembly.ForeColor), txtDisassembly.GetItemRectangle(e.Index).Location);

            e.DrawFocusRectangle();
        }

        #endregion
    }
}
