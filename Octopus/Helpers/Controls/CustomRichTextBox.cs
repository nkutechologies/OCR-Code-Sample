using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Octopus.Helpers.Controls
{
    public class CustomRichTextBox : RichTextBox
    {
        private const int SB_VERT = 1;

        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern int GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        public CustomRichTextBox()
        {
            this.VScroll += CustomRichTextBox_VScroll;
        }

        private void CustomRichTextBox_VScroll(object sender, EventArgs e)
        {
            UpdateScrollBar();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateScrollBar();
        }

        private void UpdateScrollBar()
        {
            int min, max;
            GetScrollRange(this.Handle, SB_VERT, out min, out max);

            int thumbSize = this.ClientSize.Height;

            if (max > thumbSize)
            {
                int pos = GetScrollPos(this.Handle, SB_VERT);
                SetScrollPos(this.Handle, SB_VERT, pos, true);

                int smallChange = Math.Max(1, thumbSize / 10);
                int largeChange = thumbSize;

                int minPos = 0;
                SetScrollRange(this.Handle, SB_VERT, ref minPos, ref max, true);

               


                // Customize the scrollbar's appearance here
              //  int scrollBarWidth = 5; // Adjust the width as needed
                SetWindowTheme(this.Handle, "", ""); // Ensure visual styles are applied
             //   SendMessage(this.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(scrollBarWidth << 16));

                // Set the color of the scrollbar
                SetScrollbarColor(this.Handle, ColorTranslator.ToWin32(System.Drawing.ColorTranslator.FromHtml("#392b65")));
            }
        }

        private const int EM_SETMARGINS = 211;
        private const int EC_LEFTMARGIN = 1;
        private const int EC_RIGHTMARGIN = 2;

        private enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x0001,
            SIF_PAGE = 0x0002,
            SIF_POS = 0x0004,
            SIF_DISABLENOSCROLL = 0x0008,
            SIF_TRACKPOS = 0x0010,
            SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public uint cbSize;
            public ScrollInfoMask fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetScrollbarColor(IntPtr hWnd, int color);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetScrollRange(IntPtr hWnd, int nBar, ref int nMinPos, ref int nMaxPos, bool bRedraw);


    }
}
