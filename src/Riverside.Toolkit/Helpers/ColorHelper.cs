using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Cube.UI.Helpers
{
    public class ColorHelper
    {
        private static
        (
            Color SystemAccentColorLight3,
            Color SystemAccentColorLight2,
            Color SystemAccentColorLight1,
            Color SystemAccentColor,
            Color SystemAccentColorDark1,
            Color SystemAccentColorDark2,
            Color SystemAccentColorDark3
        )
        GetColorPalette(Color c)
        {
            return
            (
                ChangeColorBrightness(c, 0.51f),
                ChangeColorBrightness(c, 0.25f),
                ChangeColorBrightness(c, 0.02f),
                c,
                ChangeColorBrightness(c, -0.19f),
                ChangeColorBrightness(c, -0.40f),
                ChangeColorBrightness(c, -0.68f)
            );
        }

        public static Color GetLighterColor1(Color c) => ChangeColorBrightness(c, 0.02f);
        public static Color GetLighterColor2(Color c) => ChangeColorBrightness(c, 0.25f);
        public static Color GetLighterColor3(Color c) => ChangeColorBrightness(c, 0.51f);
        public static Color GetLighterColor1(SolidColorBrush c) => ChangeColorBrightness(c.Color, 0.02f);
        public static Color GetLighterColor2(SolidColorBrush c) => ChangeColorBrightness(c.Color, 0.25f);
        public static Color GetLighterColor3(SolidColorBrush c) => ChangeColorBrightness(c.Color, 0.51f);

        private static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red = correctionFactor;
                green = correctionFactor;
                blue = correctionFactor;
            }
            else
            {
                red = ((255 - red) * correctionFactor) + red;
                green = ((255 - green) * correctionFactor) + green;
                blue = ((255 - blue) * correctionFactor) + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
        public static SolidColorBrush GetAccentColor() => GetColorFromHex(Application.Current.Resources["SystemAccentColor"].ToString());
        public static SolidColorBrush GetColorFromHex(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16),
                Convert.ToByte(hexaColor.Substring(7, 2), 16)
            ));
        }
    }
}
