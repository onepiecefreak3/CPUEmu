using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CPUEmu
{
    public static class RichTextBoxExtensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);
        public static void SetLineSpacing(this RichTextBox rtb, int space)
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = 0x100;

            fmt.dyLineSpacing = space;
            fmt.bLineSpacingRule = 4;

            rtb.SelectAll();
            SendMessage(new HandleRef(rtb, rtb.Handle),
                         1095,
                         1,
                         ref fmt
                       );
            rtb.Select(0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
            // PARAFORMAT2 from here onwards
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
    }
}
