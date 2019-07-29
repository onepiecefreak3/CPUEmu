using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CPUEmu
{
    class BetterListBox : ListBox
    {
        public event EventHandler Scrolled;

        private const int WM_VSCROLL = 0x115; // Vertical scroll
        private const int WM_MOUSEWHEEL = 0x20a; // Mouse wheel

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                Scrolled?.Invoke(this, new EventArgs());
            }
        }

        public new void RefreshItems()
        {
            base.RefreshItems();
        }
    }
}
