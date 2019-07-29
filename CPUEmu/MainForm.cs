using System;
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

        public MainForm()
        {
            InitializeComponent();
        }

        #region Methods

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

        #endregion

        #region UI

        private void SetupUiOpenFile()
        {
            txtlog.Clear();

            btnStartExecution.Enabled = true;
            btnStop.Enabled = false;
            btnAbort.Enabled = false;
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
            if (_adapter.Executor.IsHalted)
            {

            }
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

        private void BtnStartExecution_Click(object sender, EventArgs e)
        {
            // TODO: Choose interrupter in GUI
            _adapter.Environment.InterruptBroker = new DefaultInterruptBroker(new DefaultLogger(txtlog));
            _adapter.Executor.ExecuteAsync();
        }

        #endregion

        private void ResetUi()
        {
            txtlog.Clear();
            txtFlags.Clear();
            txtRegs.Clear();
            txtDisassembly.Items.Clear();
            // TODO: Clear breakpoint graphics
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

        #region Events

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
            LoadAdapter();
            LoadDisassembly();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleHaltExecution();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            AbortExecution();
        }

        //private void btnStep_Click(object sender, EventArgs e)
        //{
        //    DoStep();
        //}

        private void btnReExec_Click(object sender, EventArgs e)
        {
            InitializeUI();
            StartExecution();
        }

        //private void btnPrintToggle_Click(object sender, EventArgs e)
        //{
        //    TogglePrinting(!_printing);
        //}

        private void txtDisassembly_SizeChanged(object sender, EventArgs e)
        {
            RecalculateDisassemblyLines();
            //RefreshDisassembly();
            HighlightCurrentInstruction();
        }
        #endregion

        #region Methods



        private void InitializeUI()
        {
            txtlog.Clear();
            txtDisassembly.Items.Clear();

            btnStartExecution.Enabled = true;
            btnStop.Enabled = true;
            btnResume.Enabled = true;
            btnAbort.Enabled = true;
        }

        private void StartExecution()
        {
            _adapter.Executor.ExecuteAsync();
        }

        private void ToggleHaltExecution()
        {
            if (_adapter.Executor.IsHalted)
                ResumeExecution();
            else
                HaltExecution();
        }

        private void ResumeExecution()
        {
            _adapter.Executor.ResumeExecution();

            btnStop.Text = "Halt Execution";
        }

        private void HaltExecution()
        {
            _adapter.Executor.HaltExecution();

            btnStop.Text = "Resume Execution";

            // TODO: Refresh disassembly
            //RefreshDisassembly();
            HighlightCurrentInstruction();
        }

        private void AbortExecution()
        {
            _adapter.Executor.AbortExecution();

            btnAbort.Enabled = false;
            btnStop.Enabled = false;
            btnStartExecution.Enabled = true;
        }

        //private void FinishExecution()
        //{
        //    timExecution.Stop();
        //    //timTable.Stop();
        //    //timDisassembly.Stop();

        //    //RefreshFlags();
        //    //RefreshRegisters();
        //    RefreshDisassembly();
        //    HighlightCurrentInstruction();

        //    btnAbort.Enabled = false;
        //    btnStop.Enabled = false;
        //    btnStep.Enabled = false;
        //    btnReExec.Enabled = true;

        //    _stopped = false;
        //    _doStep = false;

        //    if (_abort)
        //        Log("Aborted.");
        //    else
        //        Log("Finished.");

        //    _abort = false;
        //}

        //private void DoStep()
        //{
        //    timExecution.Start();

        //    _doStep = true;
        //    _break = false;

        //    //while (_doStep || _instructionRunning)
        //    //    ;

        //    //RefreshFlags();
        //    //RefreshRegisters();

        //    //RefreshDisassembly();
        //}

        //private void TogglePrinting(bool state)
        //{
        //    if (state)
        //        EnablePrinting();
        //    else
        //        DisablePrinting();
        //}

        //private void DisablePrinting()
        //{
        //    _printing = false;

        //    btnPrintToggle.Text = "Enable Printing";
        //}

        //private void EnablePrinting()
        //{
        //    _printing = true;

        //    btnPrintToggle.Text = "Disable Printing";
        //}

        private void RefreshFlags()
        {
            if (_adapter != null)
            {
                var flags = _adapter.Environment.CpuState.GetFlags();
                var str = flags.Aggregate("", (a, b) => a += $"{b.Key}: 0x{b.Value:X8}" + Environment.NewLine);
                txtFlags.Text = str;
            }
        }

        private void RefreshRegisters()
        {
            if (_adapter != null)
            {
                var flags = _adapter.Environment.CpuState.GetRegisters();
                var str = flags.Aggregate("", (a, b) => a += $"{b.Key}: 0x{b.Value:X8}" + Environment.NewLine);
                txtRegs.Text = str;
            }
        }

        private void HighlightCurrentInstruction()
        {
            // TODO: Highlight current instruction
            //var list = GetDisassembledInstructions().ToList();

            //var currentLineIndex = list.FindIndex(x => x.count == _adapter.Executor.CurrentInstruction);
            //var selectionStart = list.TakeWhile((x, i) => i < currentLineIndex).Sum(x => CreateDisassemblyLine(x.offset, x.source).Length) - currentLineIndex;

            //if (currentLineIndex >= 0)
            //{
            //    txtDisassembly.Select(selectionStart, CreateDisassemblyLine(list[currentLineIndex].offset, list[currentLineIndex].source).Length - 1);
            //    txtDisassembly.SelectionBackColor = Color.Yellow;
            //    txtDisassembly.Select(0, 0);
            //}
        }

        private void RecalculateDisassemblyLines()
        {
            // TODO: Recalculate disassembly lines?
            //_bufferDisassemblyLines = (int)(txtDisassembly.Height / TwipsToPixel(_lineSpacingInTwips));
        }
        #endregion

        #region Disassembly
        //private static Func<long, string, string> CreateDisassemblyLine => new Func<long, string, string>((l, s) => $"{l:X4}: {s}" + Environment.NewLine);

        //private IEnumerable<(int count, long offset, string source)> GetDisassembledInstructions()
        //    => _emu?.DisassembleInstructions(_emu.CurrentInstruction, _bufferDisassemblyLines / 2, (int)Math.Ceiling(_bufferDisassemblyLines / 2d));

        //private void RefreshDisassembly()
        //{
        //    //Get disassembly range
        //    var list = GetDisassembledInstructions().ToList();

        //    //Handle disassembled data
        //    var str = list.Aggregate("", (a, b) => a + CreateDisassemblyLine(b.offset, b.source)).TrimEnd(Environment.NewLine.ToArray());
        //    _bufferedDisassemblyCounts = list.Select(x => x.count).ToArray();

        //    //Write disassembled data
        //    txtDisassembly.Text = str;
        //    txtDisassembly.SetLineSpacing(_lineSpacingInTwips);

        //    //Draw breakpoints
        //    //if (_stopped)
        //    DrawBreakpoints();
        //}
        #endregion

        private void HandleBreakpointByLine(int lineNumber)
        {
            var absoluteLine = txtDisassembly.TopIndex + lineNumber;
            _adapter.Executor.SetBreakpoint(_adapter.Instructions[absoluteLine]);
        }

        private void DrawBreakpoints()
        {
            var gr = pnBreakPoints.CreateGraphics();
            gr.Clear(pnBreakPoints.BackColor);

            var activeBreakpoints = _adapter.Executor.GetActiveBreakpoints().ToArray();
            for (int i = 0; i < txtDisassembly.Height / txtDisassembly.ItemHeight; i++)
            {
                if (activeBreakpoints.Contains(txtDisassembly.Items[i + txtDisassembly.TopIndex]))
                {
                    gr.FillEllipse(new SolidBrush(_breakPointColor),
                        new Rectangle(0, i * (int)TwipsToPixel(LineSpacingInTwips_) + 5, _breakPointSize, _breakPointSize));
                }
            }

            gr.Dispose();
            pnBreakPoints.Update();

            //var itemRange = txtDisassembly.TopIndex;
            //breakPointsInRange = active;
            // TODO: Draw breakpoint
            //if (_emu != null)
            //{
            //    var bufferedBk = _bufferedDisassemblyCounts.Select((x, i) => new { count = x, index = i }).ToArray();

            //    var bpToDraw = bufferedBk.Where(x => _breakPoints.Contains(x.count)).Select(x => x.index).ToList();

            //    var gr = pnBreakPoints.CreateGraphics();
            //    gr.Clear(pnBreakPoints.BackColor);
            //    foreach (var bp in bpToDraw)
            //        gr.FillEllipse(new SolidBrush(_breakPointColor), new Rectangle(0, bp * (int)TwipsToPixel(_lineSpacingInTwips) + 5, _breakPointSize, _breakPointSize));
            //}
        }

        private void pnBreakPoints_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentFileStream == null)
                return;

            var currentLine = (int)Math.Floor(e.Y / TwipsToPixel(LineSpacingInTwips_));
            HandleBreakpointByLine(currentLine);
            DrawBreakpoints();
        }

        //private void timTable_Tick(object sender, EventArgs e)
        //{
        //    RefreshFlags();
        //    RefreshRegisters();
        //}

        //private void timDisassembly_Tick(object sender, EventArgs e)
        //{
        //    RefreshDisassembly();
        //    HighlightCurrentInstruction();
        //}

        //private void timExecution_Tick(object sender, EventArgs e)
        //{
        //    if ((!_emu?.IsFinished ?? false) && !_abort && (!_stopped || _doStep))
        //    {
        //        //check if breakpoint is reached
        //        if (_breakPoints.Contains(_emu?.CurrentInstruction ?? 0) && !_doStep && !_break)
        //        {
        //            _break = true;
        //            _stopped = true;
        //            HaltExecution();
        //        }

        //        if (!_stopped || _doStep)
        //        {
        //            _instructionRunning = true;

        //            if (_doStep) _doStep = false;
        //            if (_break) _break = false;

        //            _emu?.ExecuteNextInstruction();

        //            _instructionRunning = false;

        //            RefreshFlags();
        //            RefreshRegisters();
        //            RefreshDisassembly();
        //            HighlightCurrentInstruction();
        //        }
        //    }

        //    if (_abort || (_emu?.IsFinished ?? false))
        //        FinishExecution();
        //}

        private void TxtDisassembly_TopIndexChanged(object sender, EventArgs e)
        {
            DrawBreakpoints();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _adapter?.Executor?.AbortExecution();
            _adapter?.Dispose();
        }

        private void BtnResume_Click(object sender, EventArgs e)
        {
            _adapter.Executor.ResumeExecution();
        }
    }
}
