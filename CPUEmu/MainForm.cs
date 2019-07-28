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

        private static double TwipsToPixel(int twips) => (int)(twips / 14.9999999999);

        private int _lineSpacingInTwips = 210;
        //private int _timeOut = 40;
        private Color _breakPointColor = Color.Red;

        //private int _bufferDisassemblyLines;
        //private int[] _bufferedDisassemblyCounts;

        //private int _breakPointSize;
        //private List<long> _breakPoints = new List<long>();

        //private Emulator _emu;

        //private bool _abort;
        //private bool _stopped;
        //private bool _break;
        //private bool _doStep;
        //private bool _instructionRunning;

        //private bool _printing;

        //System.Timers.Timer _refreshTimer;
        //System.Timers.Timer _disassembleTimer;

        public MainForm()
        {
            InitializeComponent();

            //timExecution.Interval = _timeOut;

            //txtDisassembly.Location = new Point((int)TwipsToPixel(_lineSpacingInTwips), 0);
            //txtDisassembly.Width = pnBreakPoints.Width - (int)TwipsToPixel(_lineSpacingInTwips);

            //txtDisassembly.SizeChanged += txtDisassembly_SizeChanged;

            //_bufferDisassemblyLines = (int)(txtDisassembly.Height / TwipsToPixel(_lineSpacingInTwips));
            //_breakPointSize = Math.Max(1, (int)TwipsToPixel(_lineSpacingInTwips) - 3);

            //EnablePrinting();
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

                SelectAdapter();
                _adapter.Load(_currentFileStream);
                SetupExecutorEvents();
                InitializeUI();
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

        // ----------------------------------

        private void ResetUi()
        {
            txtlog.Clear();
            txtFlags.Clear();
            txtRegs.Clear();
            txtDisassembly.Clear();
            // TODO: Clear breakpoint graphics
        }

        private void SetupExecutorEvents()
        {
            _adapter.Executor.ExecutionFinished += Executor_ExecutionFinished;
        }

        private void Executor_ExecutionFinished(object sender, EventArgs e)
        {
            ;
        }

        #region Events

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
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
            txtDisassembly.Clear();

            btnStartExecution.Enabled = true;
            btnStop.Enabled = true;
            btnAbort.Enabled = true;

            btnStop.Text = "Stop Execution";
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

        private void HandleBreakpointByLine(int line)
        {
            // TODO: Handle breakpoint by line
            //if (_emu != null)
            //{
            //    var bufferedBk = _bufferedDisassemblyCounts.ToArray();

            //    line = Math.Min(Math.Max(0, line), bufferedBk.Length - 1);
            //    var count = bufferedBk[line];

            //    if (_breakPoints.Contains(count))
            //        _breakPoints.Remove(count);
            //    else
            //        _breakPoints.Add(count);
            //}
        }

        private void DrawBreakpoints()
        {
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
            // TODO: Set breakpoint by mouse
            //if (e.X < _breakPointSize)
            //{
            //    var currentLine = (int)Math.Floor(e.Y / TwipsToPixel(_lineSpacingInTwips));
            //    HandleBreakpointByLine(currentLine);
            //    DrawBreakpoints();
            //    //RefreshDisassembly();
            //}
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

        private void BtnStartExecution_Click(object sender, EventArgs e)
        {
            btnStartExecution.Enabled = false;
            _adapter.Environment.InterruptBroker = new DefaultInterruptBroker(new DefaultLogger(txtlog));
            _adapter.Executor.ExecuteAsync();
        }
    }
}
