using System;
using System.Runtime.InteropServices;
using Riverside.ComponentModel;
using Windows.Foundation;
using Windows.Graphics;
using WinUIEx;

namespace Riverside.Toolkit.Helpers
{
    public static class NativeHelper
    {
        public static RectInt32[] Subtract(this RectInt32 rect1, RectInt32 rect2)
        {
            // Calculate the intersection
            int intersectX = Math.Max(rect1.X, rect2.X);
            int intersectY = Math.Max(rect1.Y, rect2.Y);
            int intersectWidth = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width) - intersectX;
            int intersectHeight = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - intersectY;

            // If there's no intersection, return rect1 as the only remaining rectangle
            if (intersectWidth <= 0 || intersectHeight <= 0)
            {
                return new[] { rect1 };
            }

            // Create an array to hold up to 4 rectangles
            RectInt32[] results = new RectInt32[4];
            int count = 0;

            // Top rectangle
            if (rect1.Y < intersectY)
            {
                results[count++] = new RectInt32(rect1.X, rect1.Y, rect1.Width, intersectY - rect1.Y);
            }

            // Bottom rectangle
            if (rect1.Y + rect1.Height > intersectY + intersectHeight)
            {
                results[count++] = new RectInt32(rect1.X, intersectY + intersectHeight, rect1.Width,
                    rect1.Y + rect1.Height - (intersectY + intersectHeight));
            }

            // Left rectangle
            if (rect1.X < intersectX)
            {
                results[count++] = new RectInt32(rect1.X, intersectY, intersectX - rect1.X, intersectHeight);
            }

            // Right rectangle
            if (rect1.X + rect1.Width > intersectX + intersectWidth)
            {
                results[count++] = new RectInt32(intersectX + intersectWidth, intersectY,
                    rect1.X + rect1.Width - (intersectX + intersectWidth), intersectHeight);
            }

            // Return only the non-empty results
            RectInt32[] finalResults = new RectInt32[count];
            Array.Copy(results, finalResults, count);

            return finalResults;
        }

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

        // Virtual key code for the left mouse button
        public const int VK_LBUTTON = 0x01;

        // Import the GetAsyncKeyState function from user32.dll
        [DllImport(Libraries.User32)]
        public static extern short GetAsyncKeyState(int vKey);

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
