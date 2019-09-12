using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;
using CPUEmu.Defaults;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    public partial class MainForm : Form
    {
        private readonly PluginLoader _pluginLoader = PluginLoader.Instance;
        private readonly ILogger _logger;

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

            _logger = new DefaultLogger(txtlog);
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
            _pluginLoader.SetLogger(_logger);

            // TODO: Properly select adapters dynamically
            _adapter = _pluginLoader.CreateAssemblyAdapter("TestArm");
        }

        private void LoadAdapter()
        {
            _adapter.Load(_currentFileStream);
            hexBox.ByteProvider = new MemoryMapByteProvider(_adapter.Environment.MemoryMap);
            hexBox.Refresh();
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

        private void SetCurrentInstruction(int index)
        {
            if (txtDisassembly.InvokeRequired)
                txtDisassembly.Invoke(new MethodInvoker(() => SetCurrentInstruction(index)));
            else
                txtDisassembly.SetCurrentInstructionIndex(index);
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
                hexBox.Refresh();
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
                hexBox.Refresh();
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
                hexBox.Refresh();
            }
        }

        #endregion

        #region Events

        private void Executor_ExecutionHalted(object sender, InstructionExecuteEventArgs e)
        {
            SetupUiExecutionHalted();
            LoadFlagsAndRegisters();
            SetCurrentInstruction(e.Index);
            WriteLogLine("Execution halted.");
        }

        private void Executor_ExecutionAborted(object sender, EventArgs e)
        {
            SetupUiExecutionAborted();
            LoadFlagsAndRegisters();
            SetCurrentInstruction(-1);
            WriteLogLine("Execution aborted.");
        }

        private void Executor_InstructionExecuted(object sender, InstructionExecuteEventArgs e)
        {
        }

        private void Executor_InstructionExecuting(object sender, InstructionExecuteEventArgs e)
        {
            if (_adapter.Executor.IsHalted)
                SetCurrentInstruction(e.Index);
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
            SetCurrentInstruction(-1);
            WriteLogLine("Execution finished normally.");
        }

        private void Executor_BreakpointReached(object sender, InstructionExecuteEventArgs e)
        {
            SetupUiExecutionHalted();
            LoadFlagsAndRegisters();
            SetCurrentInstruction(e.Index);
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
            _adapter.Environment.InterruptBroker = new DefaultInterruptBroker(_logger);
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

        private void TxtDisassembly_BreakpointAdded(object sender, IndexEventArgs e)
        {
            _adapter?.Executor.SetBreakpoint(_adapter.Instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointDisabled(object sender, IndexEventArgs e)
        {
            _adapter?.Executor.DisableBreakpoint(_adapter.Instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointEnabled(object sender, IndexEventArgs e)
        {
            _adapter?.Executor.EnableBreakpoint(_adapter.Instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointRemoved(object sender, IndexEventArgs e)
        {
            _adapter?.Executor.RemoveBreakpoint(_adapter.Instructions[e.AbsoluteIndex]);
        }

        #endregion
    }
}
