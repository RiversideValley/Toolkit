using System;
using System.Runtime.InteropServices;
using Riverside.ComponentModel;
using Windows.Foundation;
using WinUIEx;

namespace Riverside.Toolkit.Helpers
{
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

        [DllImport(Libraries.User32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(Libraries.User32)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(Libraries.GraphicsDeviceInterface)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public static int GET_X_LPARAM(IntPtr lParam) => unchecked((short)(long)lParam);

        public static int GET_Y_LPARAM(IntPtr lParam) => unchecked((short)((long)lParam >> 16));

        public static class Display
        {
            public static double Scale(WindowEx win)
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
