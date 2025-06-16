using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils
{
    [SupportedOSPlatform("windows")]
    public static class PlatformHelpers
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static void ValidateWin32CLI()
        {
            // when run through explorer, the window title will be set to the file path by explorer.
            // however, when run through a command line, no window title is set.
            // this reliably works across cmd, powershell, cmder etc, despite looking weird.
            if (!string.IsNullOrEmpty(Process.GetCurrentProcess().MainWindowTitle))
            {
                _ = MessageBox(IntPtr.Zero, "This program must be run from a CLI!", "Invalid Execution Environment", 0);
            }
            return;
        }
    }
}
