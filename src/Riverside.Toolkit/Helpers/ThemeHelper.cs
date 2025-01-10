using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;

namespace Cube.UI.Helpers
{
    public class ThemeHelper
    {
        public async static void LoadThemeFileAsync(string Location)
        {
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await InstallationFolder.GetFileAsync(Location);
            LoadTheme(await FileIO.ReadTextAsync(file));
        }

        public static void LoadTheme(string Theme) => Application.Current.Resources.MergedDictionaries.Add((ResourceDictionary)XamlReader.Load(Theme));
    }
}
