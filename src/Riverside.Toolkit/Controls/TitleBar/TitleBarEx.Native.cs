using Riverside.ComponentModel;
using System;
using System.Runtime.InteropServices;

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        // Importing PostMessage from user32.dll to send a message to the specified window
        [LibraryImport(Libraries.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}