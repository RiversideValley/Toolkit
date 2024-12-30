using System.Threading.Tasks;
using static Riverside.Toolkit.Helpers.NativeHelper;
using Windows.Graphics;

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        private async void LoadBounds()
        {
            // Make sure the loop doesn't trigger too often
            await Task.Delay(100);

            try
            {
                // If the window has been closed, break the loop
                if (closed) return;

                // Check if every condition is met
                if (CurrentWindow.AppWindow is not null && IsAutoDragRegionEnabled)
                {
                    // Width (Scaled window width)
                    var width = (int)(CurrentWindow.Bounds.Width * Display.Scale(CurrentWindow));

                    // Height (Scaled control actual height)
                    var height = (int)((ActualHeight + buttonDownHeight) * Display.Scale(CurrentWindow));

                    // Set the drag region for the window's title bar
                    CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new RectInt32(0, 0, width, height)]);
                }

                // Recursive call to keep updating bounds
                LoadBounds();
            }
            catch
            {
                // Silent catch to ensure the recursive loop continues
                LoadBounds();
                return;
            }
        }
    }
}