#if !Wpf

using Windows.Storage;

namespace Riverside.Toolkit.Helpers;

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

#endif