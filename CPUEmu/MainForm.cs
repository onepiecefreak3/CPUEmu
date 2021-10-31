using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.Executor;
using CPUEmu.Defaults;
using CPUEmu.Forms;
using CPUEmu.MemoryManipulation;
using Serilog;

namespace CPUEmu
{
    public partial class MainForm : Form
    {
        private readonly MemoryManipulationForm _memManipulationForm;
        private readonly IList<IMemoryManipulation> _memoryManipulations;

        private readonly PluginLoader _pluginLoader;

        private IList<IInstruction> _instructions;
        private IExecutor _executor;
        private DeviceEnvironment _deviceEnvironment;

        private string _currentFileName;
        private Stream _currentFileStream;

        public MainForm()
        {
            InitializeComponent();

            _memManipulationForm = new MemoryManipulationForm();
            _memoryManipulations = new List<IMemoryManipulation>();

            var logger = new LoggerConfiguration().WriteTo.Sink(new RichTextBoxSink(txtlog)).CreateLogger();
            _pluginLoader = new PluginLoader(logger, "plugins");
        }

        #region Methods

        #region Execution

        private void StartExecution()
        {
            // Reset environment
            _deviceEnvironment.Reset();

            // Manipulate memory
            foreach (var memoryManipulation in _memoryManipulations)
            {
                memoryManipulation.Execute(_deviceEnvironment.MemoryMap);
            }

            // Execute instructions async
            _executor.ExecuteAsync(_deviceEnvironment, 0);
        }

        private void ResumeExecution()
        {
            _executor.ResumeExecution();
            SetupUiExecutionStart();
        }

        private void HaltExecution()
        {
            _executor.HaltExecution();
        }

        private void AbortExecution()
        {
            _executor.AbortExecution();
        }

        #endregion

        #region File

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
            ResetUi();

            _currentFileStream?.Close();
            _currentFileName = null;

            _instructions = null;

            _deviceEnvironment?.Dispose();
            _deviceEnvironment = null;

            _executor.Dispose();
            _executor = null;
        }

        #endregion
        
        private IAssembly SelectAssemblyParser(Stream assembly)
        {
            foreach (var parser in _pluginLoader.EnumerateServices<IAssembly>())
            {
                assembly.Position = 0;
                if (!parser.CanIdentify)
                    continue;

                if (parser.Identify(assembly))
                    return parser;
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
            _executor.ExecutionStepped += Executor_ExecutionStepped;

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

                txtFlags.Items.AddRange(_executor.CpuState.GetFlags().Select(x => (object)new FlagRegisterItem(x.Key, x.Value)).ToArray());
                txtRegisters.Items.AddRange(_executor.CpuState.GetRegisters().Select(x => (object)new FlagRegisterItem(x.Key, x.Value)).ToArray());
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

            _executor?.ResetBreakpoints();
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

            hexBox.ByteProvider = new MemoryMapByteProvider(_deviceEnvironment.MemoryMap);
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
                txtDisassembly.SetCurrentInstructionIndex(-1);

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
            LoadFlagsAndRegisters();
            SetCurrentInstruction(e.Index);
            WriteLogLine("Execution halted.");
            SetupUiExecutionHalted();
        }

        private void Executor_ExecutionAborted(object sender, EventArgs e)
        {
            LoadFlagsAndRegisters();
            WriteLogLine("Aborted on instruction: " + _executor.CurrentInstruction);
            SetCurrentInstruction(-1);
            WriteLogLine("Execution aborted.");
            SetupUiExecutionAborted();
        }

        private void Executor_ExecutionStepped(object sender, InstructionExecuteEventArgs e)
        {
            LoadFlagsAndRegisters();
            SetCurrentInstruction(e.Index);
            SetupUiExecutionHalted();
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
            LoadFlagsAndRegisters();
            WriteLogLine("Execution started.");
            SetupUiExecutionStart();
        }

        private void Executor_ExecutionFinished(object sender, EventArgs e)
        {
            LoadFlagsAndRegisters();
            SetCurrentInstruction(-1);
            WriteLogLine("Execution finished normally.");
            SetupUiExecutionFinished();
        }

        private void Executor_BreakpointReached(object sender, InstructionExecuteEventArgs e)
        {
            LoadFlagsAndRegisters();
            SetCurrentInstruction(e.Index);
            WriteLogLine("Breakpoint reached.");
            SetupUiExecutionHalted();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileStream = OpenFile();
            if (fileStream != null)
            {
                LoadAssembly(fileStream);
                if (_instructions == null || _deviceEnvironment == null)
                {
                    fileStream.Close();
                    return;
                }

                // Prepare UI and load file globally
                _currentFileName = fileStream.Name;
                _currentFileStream = fileStream;

                _deviceEnvironment.Reset();
                SetupUiOpenFile();
                SetupExecutorEvents();

                LoadDisassembly();
            }
        }

        private void LoadAssembly(Stream assembly, string parserName = null)
        {
            // Get assembly parser
            var assemblyParser = !string.IsNullOrEmpty(parserName) ?
                _pluginLoader.GetService<IAssembly>(parserName) :
                SelectAssemblyParser(assembly);

            // TODO: Make unidentifiable parser selectable?
            if (assemblyParser == null)
                return;

            // Get architecture provider
            var architectureProvider = _pluginLoader.GetService<IArchitectureProvider>(assemblyParser.Architecture);
            if (architectureProvider == null)
                return;

            // Load instructions
            assembly.Position = 0;
            assemblyParser.LoadPayload(assembly, architectureProvider.InstructionParser);

            // Load execution environment
            assembly.Position = 0;
            _executor = architectureProvider.Executor;
            _deviceEnvironment = assemblyParser.CreateExecutionEnvironment(assembly, _executor);

            _instructions = _executor.Instructions;
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
            CloseFile();
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
            SetupUiExecutionStart();
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

            if (!int.TryParse(txtEditFlagRegister.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var value))
            {
                return;
            }

            flagRegisterItem?.SetValue(value);
            listBox.Items[listBox.SelectedIndex] = flagRegisterItem;

            if (listBox == txtFlags)
                _executor.CpuState.SetFlag(flagRegisterItem?.Name, flagRegisterItem?.Value);
            if (listBox == txtRegisters)
                _executor.CpuState.SetRegister(flagRegisterItem?.Name, flagRegisterItem?.Value);

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
            var item = (FlagRegisterItem)listBox.Items[itemSelected];

            txtEditFlagRegister.Tag = listBox;
            txtEditFlagRegister.Location = new Point(listBoxPosition.X + delta, listBoxPosition.Y + itemSelected * listBox.ItemHeight + delta);
            txtEditFlagRegister.Show();

            txtEditFlagRegister.Text = Convert.ToInt32(item.Value).ToString("X8");
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
