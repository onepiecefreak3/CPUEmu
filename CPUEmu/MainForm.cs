using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.DependencyInjection;
using CpuContract.Executor;
using CpuContract.Logging;
using CPUEmu.Defaults;
using CPUEmu.Forms;
using CPUEmu.MemoryManipulation;

namespace CPUEmu
{
    public partial class MainForm : Form
    {
        private readonly MemoryManipulationForm _memManipulationForm;
        private IList<IMemoryManipulation> _memoryManipulations;

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

            _memManipulationForm = new MemoryManipulationForm();
            _memoryManipulations = new List<IMemoryManipulation>();

            _logger = new DefaultLogger(txtlog);
            _pluginLoader = new PluginLoader(_logger, "plugins");
        }

        #region Methods

        private void StartExecution()
        {
            // Reset environment
            _executionEnvironment.Reset();

            // Manipulate memory
            foreach (var memoryManipulation in _memoryManipulations)
            {
                memoryManipulation.Execute(_executionEnvironment.MemoryMap);
            }

            // Execute instructions async
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
                txtFlags.Items.Clear();
                txtRegisters.Items.Clear();

                txtFlags.Items.AddRange(_executionEnvironment.CpuState.GetFlags().Select(x => (object)new FlagRegisterItem(x.Key, x.Value)).ToArray());
                txtRegisters.Items.AddRange(_executionEnvironment.CpuState.GetRegisters().Select(x => (object)new FlagRegisterItem(x.Key, x.Value)).ToArray());
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
            txtFlags.Items.Clear();
            txtRegisters.Items.Clear();
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
            btnAddManipulation.Enabled = true;

            hexBox.ByteProvider = new MemoryMapByteProvider(_executionEnvironment.MemoryMap);
            hexBox.Refresh();

            LoadFlagsAndRegisters();
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
                btnAddManipulation.Enabled = false;
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
                btnAddManipulation.Enabled = true;

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
                btnAddManipulation.Enabled = true;

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

                _executionEnvironment.Reset();
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

        private void BtnAddManipulation_Click(object sender, EventArgs e)
        {
            if (_memManipulationForm.ShowDialog() == DialogResult.OK)
            {
                _memoryManipulations.Add(_memManipulationForm.CurrentMemoryManipulation);
                manipulationTxt.AppendText(_memManipulationForm.CurrentMemoryManipulation + Environment.NewLine);
            }
        }

        private void TxtEditFlagRegister_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If Return was pressed
            if (e.KeyChar == 13)
                SetFlagRegisterValue();
        }

        private void TxtEditFlagRegister_Leave(object sender, EventArgs e)
        {
            SetFlagRegisterValue();
        }

        private void SetFlagRegisterValue()
        {
            var listBox = (ListBox)txtEditFlagRegister.Tag;
            var flagRegisterItem = listBox.Items[listBox.SelectedIndex] as FlagRegisterItem;
            flagRegisterItem?.SetValue(txtEditFlagRegister.Text);
            listBox.Items[listBox.SelectedIndex] = flagRegisterItem;

            if (listBox == txtFlags)
                _executionEnvironment.CpuState.SetFlag(flagRegisterItem?.Name, flagRegisterItem?.Value);
            if (listBox == txtRegisters)
                _executionEnvironment.CpuState.SetRegister(flagRegisterItem?.Name, flagRegisterItem?.Value);

            txtEditFlagRegister.Hide();
        }

        private void TxtFlags_DoubleClick(object sender, EventArgs e)
        {
            ActivateFlagRegisterEdit(sender as ListBox, 5);
        }

        private void TxtFlags_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                ActivateFlagRegisterEdit(sender as ListBox, 5);
        }

        private void ActivateFlagRegisterEdit(ListBox listBox, int delta)
        {
            if (listBox.SelectedIndex < 0)
                return;

            var listBoxPosition = Point.Empty;
            Control control = listBox;
            while (control != this)
            {
                listBoxPosition = new Point(listBoxPosition.X + control.Location.X, listBoxPosition.Y + control.Location.Y);
                control = control.Parent;
            }

            var itemSelected = listBox.SelectedIndex;
            //var formPoint = listBox.PointToScreen(Point.Empty);
            //formPoint = new Point(formPoint.X - Location.X, formPoint.Y - Location.Y);
            var item = (FlagRegisterItem)listBox.Items[itemSelected];

            txtEditFlagRegister.Tag = listBox;
            txtEditFlagRegister.Location = new Point(listBoxPosition.X + delta, listBoxPosition.Y + itemSelected * listBox.ItemHeight + delta);
            txtEditFlagRegister.Show();

            txtEditFlagRegister.Text = item.Value.ToString();
            txtEditFlagRegister.Focus();
            txtEditFlagRegister.SelectAll();
        }

        private void TxtRegisters_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                ActivateFlagRegisterEdit(sender as ListBox, 5);
        }

        private void TxtRegisters_DoubleClick(object sender, EventArgs e)
        {
            ActivateFlagRegisterEdit(sender as ListBox, 5);
        }
    }
}
