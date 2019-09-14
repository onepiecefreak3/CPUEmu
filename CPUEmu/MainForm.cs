using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.DependencyInjection;
using CpuContract.Executor;
using CpuContract.Logging;
using CPUEmu.Defaults;

namespace CPUEmu
{
    public partial class MainForm : Form
    {
        private readonly PluginLoader _pluginLoader;
        private readonly ILogger _logger;

        private IList<IInstruction> _instructions;
        private IExecutionEnvironment _executionEnvironment;
        private IExecutor _executor;

        private string _currentFileName;
        private Stream _currentFileStream;

        public MainForm()
        {
            InitializeComponent();

            _logger = new DefaultLogger(txtlog);
            _pluginLoader = new PluginLoader(_logger, "plugins");
        }

        #region Methods

        private void StartExecution()
        {
            _executionEnvironment.Reset();
            _executor.ExecuteAsync(_executionEnvironment, _instructions, 0);
        }

        private void ResumeExecution()
        {
            _executor.ResumeExecution();
        }

        private void HaltExecution()
        {
            _executor.HaltExecution();
        }

        private void AbortExecution()
        {
            _executor.AbortExecution();
        }

        private FileStream OpenFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName))
                    throw new FileNotFoundException(ofd.FileName);

                return File.OpenRead(ofd.FileName);
            }

            return null;
        }

        private void CloseFile()
        {
            _currentFileStream.Close();
            _currentFileName = null;

            _instructions.Clear();
            _instructions = null;

            _executionEnvironment.Dispose();
            _executionEnvironment = null;

            _executor.Dispose();
            _executor = null;

            ResetUi();
        }

        private IAssemblyAdapter SelectAdapter(Stream assembly, IServiceProvider<IAssemblyAdapter> adapterProvider)
        {
            foreach (var adapter in adapterProvider.EnumerateServices())
            {
                assembly.Position = 0;
                if (adapter.Identify(assembly))
                    return adapter;
                adapterProvider.ReleaseService(adapter);
            }

            return null;
        }

        private IList<IInstruction> LoadInstructions(IAssemblyAdapter adapter, Stream assembly)
        {
            try
            {
                assembly.Position = 0;
                return adapter.ParseAssembly(assembly);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Fatal, "Instructions could not be parsed.");
                _logger.Log(LogLevel.Fatal, e.Message);
            }

            return null;
        }

        private IExecutionEnvironment CreateExecutionEnvironment(IAssemblyAdapter adapter, Stream assembly)
        {
            try
            {
                assembly.Position = 0;
                return adapter.CreateExecutionEnvironment(assembly);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Fatal, "Execution environment could not be created.");
                _logger.Log(LogLevel.Fatal, e.Message);
            }

            return null;
        }

        private IExecutor CreateExecutor(IAssemblyAdapter adapter)
        {
            try
            {
                return adapter.CreateExecutor();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Fatal, "Executor could not be created.");
                _logger.Log(LogLevel.Fatal, e.Message);
            }

            return null;
        }

        private void SetupExecutorEvents()
        {
            _executor.ExecutionStarted += Executor_ExecutionStarted;
            _executor.ExecutionFinished += Executor_ExecutionFinished;

            _executor.InstructionExecuting += Executor_InstructionExecuting;
            _executor.InstructionExecuted += Executor_InstructionExecuted;

            _executor.ExecutionHalted += Executor_ExecutionHalted;
            _executor.ExecutionAborted += Executor_ExecutionAborted;

            _executor.BreakpointReached += Executor_BreakpointReached;
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
            txtDisassembly.Items.AddRange(_instructions.Cast<object>().ToArray());
        }

        private void LoadFlagsAndRegisters()
        {
            if (txtFlags.InvokeRequired)
                txtFlags.Invoke(new MethodInvoker(LoadFlagsAndRegisters));
            else
            {
                txtFlags.Text = string.Join(Environment.NewLine, _executionEnvironment.CpuState.GetFlags());
                txtRegs.Text = string.Join(Environment.NewLine, _executionEnvironment.CpuState.GetRegisters().Select(x => $"[{x.Key},{x.Value:X8}]"));
            }
        }

        private void SetCurrentInstruction(int index)
        {
            if (txtDisassembly.InvokeRequired)
                txtDisassembly.Invoke(new MethodInvoker(() => SetCurrentInstruction(index)));
            else
                txtDisassembly.SetCurrentInstructionIndex(index);
        }

        private long GetAbsolutePosition()
        {
            return hexBox.CurrentPositionInLine - 1 + (hexBox.CurrentLine - 1) * hexBox.BytesPerLine;
        }

        #endregion

        #region UI

        private void ResetUi()
        {
            txtlog.Clear();
            txtFlags.Clear();
            txtRegs.Clear();
            txtDisassembly.Items.Clear();

            _executor.ResetBreakpoints();
        }

        private void SetupUiOpenFile()
        {
            txtlog.Clear();

            btnStartExecution.Enabled = true;
            btnStop.Enabled = false;
            btnAbort.Enabled = false;
            btnResume.Enabled = false;
            btnStep.Enabled = false;

            hexBox.ByteProvider = new MemoryMapByteProvider(_executionEnvironment.MemoryMap);
            hexBox.Refresh();
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
                btnStep.Enabled = false;
                hexBox.ReadOnly = false;
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
                btnStep.Enabled = false;

                hexBox.Refresh();
                hexBox.ReadOnly = true;
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
                btnStep.Enabled = true;

                hexBox.Refresh();
            }
        }

        private void SetupUiExecutionAborted()
        {
            if (btnStartExecution.InvokeRequired)
                btnStartExecution.Invoke(new MethodInvoker(SetupUiExecutionAborted));
            else
            {
                btnStartExecution.Enabled = true;
                btnStop.Enabled = false;
                btnResume.Enabled = false;
                btnAbort.Enabled = false;
                btnStep.Enabled = false;

                hexBox.Refresh();
                hexBox.ReadOnly = true;
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
            WriteLogLine("Aborted on instruction: " + _executor.CurrentInstruction);
            SetCurrentInstruction(-1);
            WriteLogLine("Execution aborted.");
        }

        private void Executor_InstructionExecuted(object sender, InstructionExecuteEventArgs e)
        {
        }

        private void Executor_InstructionExecuting(object sender, InstructionExecuteEventArgs e)
        {
            if (_executor.IsHalted)
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
            var fileStream = OpenFile();
            if (fileStream != null)
            {
                LoadAssembly(fileStream);
                if (_instructions == null || _executionEnvironment == null || _executor == null)
                {
                    fileStream.Close();
                    return;
                }

                // Prepare UI and load file globally
                _currentFileName = fileStream.Name;
                _currentFileStream = fileStream;

                SetupUiOpenFile();
                SetupExecutorEvents();

                LoadDisassembly();
            }
        }

        private void LoadAssembly(Stream assembly)
        {
            // First try to select an adapter
            var adapterProvider = _pluginLoader.GetServiceProvider<IAssemblyAdapter>();
            var adapter = SelectAdapter(assembly, adapterProvider);
            if (adapter == null)
                return;

            // Load instructions
            var instructions = LoadInstructions(adapter, assembly);
            if (instructions == null)
                return;

            // Load execution environment
            var executionEnvironment = CreateExecutionEnvironment(adapter, assembly);
            if (executionEnvironment == null)
                return;

            // Load executor
            var executor = CreateExecutor(adapter);
            if (executor == null)
                return;

            _instructions = instructions;
            _executionEnvironment = executionEnvironment;
            _executor = executor;

            // Release adapter
            adapterProvider.ReleaseService(adapter);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void BtnStartExecution_Click(object sender, EventArgs e)
        {
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
            _instructions?.Clear();
            _instructions = null;

            _executionEnvironment?.Dispose();
            _executionEnvironment = null;

            _executor?.Dispose();
            _executor = null;
        }

        private void TxtDisassembly_BreakpointAdded(object sender, IndexEventArgs e)
        {
            _executor.SetBreakpoint(_instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointDisabled(object sender, IndexEventArgs e)
        {
            _executor.DisableBreakpoint(_instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointEnabled(object sender, IndexEventArgs e)
        {
            _executor.EnableBreakpoint(_instructions[e.AbsoluteIndex]);
        }

        private void TxtDisassembly_BreakpointRemoved(object sender, IndexEventArgs e)
        {
            _executor.RemoveBreakpoint(_instructions[e.AbsoluteIndex]);
        }

        private void HexBox_CurrentLineChanged(object sender, EventArgs e)
        {
            bytePositionTxt.Text = GetAbsolutePosition().ToString("X8");
        }

        private void HexBox_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            bytePositionTxt.Text = GetAbsolutePosition().ToString("X8");
        }

        private void BytePositionTxt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (int.TryParse(bytePositionTxt.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out var position))
                {
                    bytePositionTxt.Text = position.ToString("X8");
                    hexBox.ScrollByteIntoView(position + 1);
                }
            }
        }

        private void HexBox_ByteProviderChanged(object sender, EventArgs e)
        {
            bytePositionTxt.Enabled = bytePositionTxt.Visible = hexBox.ByteProvider != null;
        }

        #endregion

        private void BtnStep_Click(object sender, EventArgs e)
        {
            _executor.StepExecution();
        }
    }
}
