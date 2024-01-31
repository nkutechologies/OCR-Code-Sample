using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus.Helpers
{
    public class ClipboardManager
    {
        public static void Clear()
        {
            Clipboard.Clear();
        }
        public static string GetClipBoardContent()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            else
            {
                // Handle if no text is found in the clipboard
                return string.Empty;
            }
        }
    }
}
