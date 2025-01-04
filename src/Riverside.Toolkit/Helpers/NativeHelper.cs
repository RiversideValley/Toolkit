using System;
using System.Runtime.InteropServices;
using Riverside.ComponentModel;
using Windows.Foundation;
using WinUIEx;

#nullable enable

namespace Riverside.Toolkit.Helpers
{
    /// <summary>
    /// Provides helper methods for native interop.
    /// </summary>
    public static class NativeHelper
    {
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEMOVE = 0x0200;

        public const uint GW_HWNDNEXT = 2;
        public const uint GW_CHILD = 5;

        public const int HORZRES = 8; // Horizontal width of the display
        public const int VERTRES = 10; // Vertical height of the display
        public const int LOGPIXELSX = 88; // Logical pixels/inch in X

        /// <summary>
        /// Retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be retrieved.</param>
        /// <returns>The handle to the DC for the specified window's client area.</returns>
        [DllImport(Libraries.User32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases a device context (DC), freeing it for use by other applications.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
        /// <param name="hDC">A handle to the DC to be released.</param>
        /// <returns>The return value indicates whether the DC was released.</returns>
        [DllImport(Libraries.User32)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hdc">A handle to the DC.</param>
        /// <param name="nIndex">The item to be returned.</param>
        /// <returns>The value of the specified item.</returns>
        [DllImport(Libraries.GraphicsDeviceInterface)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        /// <summary>
        /// Extracts the X-coordinate from the lParam parameter of a window message.
        /// </summary>
        /// <param name="lParam">The lParam parameter of a window message.</param>
        /// <returns>The X-coordinate.</returns>
        public static int GetXFromLParam(IntPtr lParam) => unchecked((short)(long)lParam);

        /// <summary>
        /// Extracts the Y-coordinate from the lParam parameter of a window message.
        /// </summary>
        /// <param name="lParam">The lParam parameter of a window message.</param>
        /// <returns>The Y-coordinate.</returns>
        public static int GetYFromLParam(IntPtr lParam) => unchecked((short)((long)lParam >> 16));

        /// <summary>
        /// Provides methods for display-related operations.
        /// </summary>
        public static class Display
        {
            /// <summary>
            /// Gets the scale factor of the display.
            /// </summary>
            /// <param name="win">The window for which to get the scale factor.</param>
            /// <returns>The scale factor of the display.</returns>
            public static double Scale(WindowEx? win)
            {
                // Get the handle to the current window
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(win);

                // Get the device context for the window
                var hdc = GetDC(hWnd);

                // Get the DPI
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);

                // Release the device context
                _ = ReleaseDC(hWnd, hdc);

                return dpiX / 96.0;
            }

            /// <summary>
            /// Gets the rectangle representing the display area.
            /// </summary>
            /// <param name="win">The window for which to get the display rectangle.</param>
            /// <returns>The rectangle representing the display area.</returns>
            public static Rect GetDisplayRect(WindowEx win)
            {
                // Get the handle to the current window
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(win);

                // Get the device context for the window
                var hdc = GetDC(hWnd);

                // Get the width and height of the display
                var width = GetDeviceCaps(hdc, HORZRES);
                var height = GetDeviceCaps(hdc, VERTRES);

                // Release the device context
                _ = ReleaseDC(hWnd, hdc);

                return new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = width,
                    Height = height
                };
            }

            /// <summary>
            /// Gets the rectangle representing the display area, adjusted for DPI.
            /// </summary>
            /// <param name="win">The window for which to get the DPI-aware display rectangle.</param>
            /// <returns>The rectangle representing the DPI-aware display area.</returns>
            public static Rect GetDPIAwareDisplayRect(WindowEx win)
            {
                // Get the handle to the current window
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(win);

                // Get the device context for the window
                var hdc = GetDC(hWnd);

                // Get the width and height of the display
                var width = GetDeviceCaps(hdc, HORZRES) / Scale(win);
                var height = GetDeviceCaps(hdc, VERTRES) / Scale(win);

                // Release the device context
                _ = ReleaseDC(hWnd, hdc);

                return new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = width,
                    Height = height
                };
            }
        }
    }
}
