using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CPUEmu
{
    class CodingListBox : ListBox
    {
        private const int WmVscroll = 0x115; // Vertical scroll
        private const int WmMousewheel = 0x20a; // Mouse wheel

        private readonly Dictionary<int, bool> _breakpoints;

        private Color _currentInstructionColor = Color.Yellow;
        private Color _breakpointColor = Color.Red;
        private int _breakpointRadius = 8;
        private int _breakpointAreaWidth = 17;

        public Color CurrentInstructionColor
        {
            get => _currentInstructionColor;
            set
            {
                _currentInstructionColor = value;
                RefreshItems();
            }
        }

        public Color BreakpointColor
        {
            get => _breakpointColor;
            set
            {
                _breakpointColor = value;
                RefreshItems();
            }
        }

        public int BreakpointRadius
        {
            get => _breakpointRadius;
            set
            {
                _breakpointRadius = value;
                RefreshItems();
            }
        }

        public int BreakpointAreaWidth
        {
            get => _breakpointAreaWidth;
            set
            {
                _breakpointAreaWidth = value;
                RefreshItems();
            }
        }

        public int CurrentInstructionIndex { get; private set; } = -1;

        public event EventHandler<IndexEventArgs> BreakpointAdded;
        public event EventHandler<IndexEventArgs> BreakpointRemoved;
        public event EventHandler<IndexEventArgs> BreakpointEnabled;
        public event EventHandler<IndexEventArgs> BreakpointDisabled;
        public event EventHandler Scrolling;
        public event EventHandler Scrolled;

        public CodingListBox()
        {
            DrawItem += CodingListBox_DrawItem;
            MouseUp += CodingListBox_MouseUp;
            _breakpoints = new Dictionary<int, bool>();
        }

        public void SetCurrentInstructionIndex(int index)
        {
            // Remember old and new instruction index
            var previousCurrentInstruction = CurrentInstructionIndex;
            CurrentInstructionIndex = index;

            // Redraw previous item if index changed
            if (CurrentInstructionIndex != previousCurrentInstruction)
                RedrawItem(previousCurrentInstruction);

            // Jump list content to current instruction as top instruction
            SetTopIndex(index);

            // Redraw current instruction item
            RedrawItem(index);
        }

        private void SetTopIndex(int index)
        {
            var newTopIndex = Math.Max(0, Math.Min(index, Items.Count - Height / ItemHeight));
            TopIndex = newTopIndex;
        }

        public void AddBreakpoint(int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new IndexOutOfRangeException();

            _breakpoints.Add(index, true);
            OnBreakpointAdded(new IndexEventArgs(index));
            RedrawItem(index);
        }

        public void RemoveBreakpoint(int index)
        {
            if (!_breakpoints.ContainsKey(index))
                throw new KeyNotFoundException();

            _breakpoints.Remove(index);
            OnBreakpointRemoved(new IndexEventArgs(index));
            RedrawItem(index);
        }

        public void DisableBreakpoint(int index)
        {
            if (!_breakpoints.ContainsKey(index))
                throw new KeyNotFoundException();

            _breakpoints[index] = false;
            OnBreakpointDisabled(new IndexEventArgs(index));
            RedrawItem(index);
        }

        public void EnableBreakpoint(int index)
        {
            if (!_breakpoints.ContainsKey(index))
                throw new KeyNotFoundException();

            _breakpoints[index] = true;
            OnBreakpointEnabled(new IndexEventArgs(index));
            RedrawItem(index);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmVscroll || m.Msg == WmMousewheel)
                OnScrolling(new EventArgs());

            base.WndProc(ref m);

            if (m.Msg == WmVscroll || m.Msg == WmMousewheel)
                OnScrolled(new EventArgs());
        }

        protected virtual void OnBreakpointAdded(IndexEventArgs e)
        {
            BreakpointAdded?.Invoke(this, e);
        }

        protected virtual void OnBreakpointRemoved(IndexEventArgs e)
        {
            BreakpointRemoved?.Invoke(this, e);
        }

        protected virtual void OnBreakpointDisabled(IndexEventArgs e)
        {
            BreakpointDisabled?.Invoke(this, e);
        }

        protected virtual void OnBreakpointEnabled(IndexEventArgs e)
        {
            BreakpointEnabled?.Invoke(this, e);
        }

        protected virtual void OnScrolling(EventArgs e)
        {
            Scrolling?.Invoke(this, e);
        }

        protected virtual void OnScrolled(EventArgs e)
        {
            Scrolled?.Invoke(this, e);
        }

        private void RedrawItem(int index)
        {
            if (index >= TopIndex && index < TopIndex + Height / ItemHeight)
                OnDrawItem(new DrawItemEventArgs(CreateGraphics(), Font, GetItemRectangle(index), index, DrawItemState.None));
        }

        private void CodingListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var absoluteLine = Math.Min(Items.Count - 1, e.Index);
            if (absoluteLine < 0)
                return;

            // Breakpoint area
            DrawBreakpointArea(e, absoluteLine);

            // Text lines
            DrawTextLine(e, absoluteLine);

            e.DrawFocusRectangle();
        }

        private void CodingListBox_MouseUp(object sender, MouseEventArgs e)
        {
            var mouseLine = e.Y / ItemHeight;
            if (TopIndex + mouseLine >= Items.Count || e.X > _breakpointAreaWidth)
                return;

            var absoluteLine = Math.Min(Items.Count - 1, TopIndex + mouseLine);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_breakpoints.ContainsKey(absoluteLine))
                        RemoveBreakpoint(absoluteLine);
                    else
                        AddBreakpoint(absoluteLine);
                    break;
                case MouseButtons.Right:
                    if (_breakpoints.ContainsKey(absoluteLine))
                    {
                        if (_breakpoints[absoluteLine])
                            DisableBreakpoint(absoluteLine);
                        else
                            EnableBreakpoint(absoluteLine);
                    }
                    break;
            }
        }

        private void DrawBreakpointArea(DrawItemEventArgs e, int absoluteIndex)
        {
            var g = e.Graphics;

            // Draw background
            g.FillRectangle(new SolidBrush(BackColor), new Rectangle(e.Bounds.X, e.Bounds.Y, BreakpointAreaWidth, e.Bounds.Height));

            // Draw breakpoint circle, if needed
            if (_breakpoints.ContainsKey(absoluteIndex))
            {
                var breakpointRect = new Rectangle(e.Bounds.X + (BreakpointAreaWidth - BreakpointRadius) / 2,
                    e.Bounds.Y + (ItemHeight - BreakpointRadius) / 2,
                    BreakpointRadius,
                    BreakpointRadius);
                if (_breakpoints[absoluteIndex])
                    g.FillEllipse(new SolidBrush(BreakpointColor), breakpointRect);
                else
                    g.DrawEllipse(new Pen(BreakpointColor), breakpointRect);
            }

            // Draw splitting line
            g.DrawLine(new Pen(Color.LightGray), e.Bounds.X + BreakpointAreaWidth, e.Bounds.Y, e.Bounds.X + BreakpointAreaWidth, e.Bounds.Y + ItemHeight);
        }

        private void DrawTextLine(DrawItemEventArgs e, int absoluteIndex)
        {
            var g = e.Graphics;
            var textRect = new Rectangle(e.Bounds.X + BreakpointAreaWidth + 1, e.Bounds.Y,
                e.Bounds.Width - BreakpointAreaWidth - 1, e.Bounds.Height);

            // Background
            SolidBrush backgroundBrush;

            // Mark current instruction that gets executed
            if (CurrentInstructionIndex >= 0 &&
                CurrentInstructionIndex == absoluteIndex &&
                CurrentInstructionIndex >= TopIndex &&
                CurrentInstructionIndex - TopIndex < Height / ItemHeight)
                backgroundBrush = new SolidBrush(CurrentInstructionColor);
            else if (_breakpoints.ContainsKey(absoluteIndex) && _breakpoints[absoluteIndex])
                backgroundBrush = new SolidBrush(BreakpointColor);
            else
                backgroundBrush = new SolidBrush(BackColor);
            g.FillRectangle(backgroundBrush, textRect);

            // Text
            g.DrawString(Items[absoluteIndex].ToString(), e.Font, new SolidBrush(ForeColor), textRect.Location);
        }
    }
}
