#if !Wpf

using CommunityToolkit.Common.Helpers;
using CommunityToolkit.Helpers;
using Riverside.Extensions.Accountability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Path = System.IO.Path;

namespace Riverside.Toolkit.Helpers.ObjectStorage;

/// <summary>
/// Storage helper for files and folders living in Windows.Storage.ApplicationData storage endpoints.
/// </summary>
/// <param name="appData">The data store to interact with.</param>
/// <param name="objectSerializer">Serializer for converting stored values. Defaults to <see cref="SystemSerializer"/>.</param>
[NotMyCode("MIT", "https://github.com/CommunityToolkit/WindowsCommunityToolkit", ".NET Foundation", null)]
public partial class ApplicationDataStorageHelper(ApplicationData appData, IObjectSerializer? objectSerializer = null) : IFileStorageHelper, ISettingsStorageHelper<string>
{
    /// <summary>
    /// Gets the settings container.
    /// </summary>
    public ApplicationDataContainer Settings => AppData.LocalSettings;

    /// <summary>
    ///  Gets the storage folder.
    /// </summary>
    public StorageFolder Folder => AppData.LocalFolder;

    /// <summary>
    /// Gets the storage host.
    /// </summary>
    protected ApplicationData AppData { get; } = appData ?? throw new ArgumentNullException(nameof(appData));

    /// <summary>
    /// Gets the serializer for converting stored values.
    /// </summary>
    protected IObjectSerializer Serializer { get; } = objectSerializer ?? new SystemSerializer();

    /// <summary>
    /// Get a new instance using ApplicationData.Current and the provided serializer.
    /// </summary>
    /// <param name="objectSerializer">Serializer for converting stored values. Defaults to <see cref="SystemSerializer"/>.</param>
    /// <returns>A new instance of ApplicationDataStorageHelper.</returns>
    public static ApplicationDataStorageHelper GetCurrent(IObjectSerializer? objectSerializer = null)
    {
        var appData = ApplicationData.Current;
        return new ApplicationDataStorageHelper(appData, objectSerializer);
    }

#if !Uno
    /// <summary>
    /// Get a new instance using the ApplicationData for the provided user and serializer.
    /// </summary>
    /// <param name="user">App data user owner.</param>
    /// <param name="objectSerializer">Serializer for converting stored values. Defaults to <see cref="SystemSerializer"/>.</param>
    /// <returns>A new instance of ApplicationDataStorageHelper.</returns>
    public static async Task<ApplicationDataStorageHelper> GetForUserAsync(User user, IObjectSerializer? objectSerializer = null)
    {
        var appData = await ApplicationData.GetForUserAsync(user);
        return new ApplicationDataStorageHelper(appData, objectSerializer);
    }
#endif

    /// <summary>
    /// Determines whether a setting already exists.
    /// </summary>
    /// <param name="key">Key of the setting (that contains object).</param>
    /// <returns>True if a value exists.</returns>
    public bool KeyExists(string key)
    {
        return Settings.Values.ContainsKey(key);
    }

    /// <summary>
    /// Retrieves a single item by its key.
    /// </summary>
    /// <typeparam name="T">Type of object retrieved.</typeparam>
    /// <param name="key">Key of the object.</param>
    /// <param name="default">Default value of the object.</param>
    /// <returns>The TValue object.</returns>
    public T? Read<T>(string key, T? @default = default)
    {
        if (Settings.Values.TryGetValue(key, out var valueObj) && valueObj is string valueString)
        {
            return Serializer.Deserialize<T>(valueString);
        }

        return @default;
    }

    /// <inheritdoc />
    public bool TryRead<T>(string key, out T? value)
    {
        if (Settings.Values.TryGetValue(key, out var valueObj) && valueObj is string valueString)
        {
            value = Serializer.Deserialize<T>(valueString);
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public void Save<T>(string key, T value)
    {
        Settings.Values[key] = Serializer.Serialize(value);
    }

    /// <inheritdoc />
    public bool TryDelete(string key)
    {
        return Settings.Values.Remove(key);
    }

    /// <inheritdoc />
    public void Clear()
    {
        Settings.Values.Clear();
    }

    /// <summary>
    /// Determines whether a setting already exists in composite.
    /// </summary>
    /// <param name="compositeKey">Key of the composite (that contains settings).</param>
    /// <param name="key">Key of the setting (that contains object).</param>
    /// <returns>True if a value exists.</returns>
    public bool KeyExists(string compositeKey, string key)
    {
        if (TryRead(compositeKey, out ApplicationDataCompositeValue? composite) && composite != null)
        {
            return composite.ContainsKey(key);
        }

        return false;
    }

    /// <summary>
    /// Attempts to retrieve a single item by its key in composite.
    /// </summary>
    /// <typeparam name="T">Type of object retrieved.</typeparam>
    /// <param name="compositeKey">Key of the composite (that contains settings).</param>
    /// <param name="key">Key of the object.</param>
    /// <param name="value">The value of the object retrieved.</param>
    /// <returns>The T object.</returns>
    public bool TryRead<T>(string compositeKey, string key, out T? value)
    {
        if (TryRead(compositeKey, out ApplicationDataCompositeValue? composite) && composite != null)
        {
            string compositeValue = (string)composite[key];
            if (compositeValue != null)
            {
                value = Serializer.Deserialize<T>(compositeValue);
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Retrieves a single item by its key in composite.
    /// </summary>
    /// <typeparam name="T">Type of object retrieved.</typeparam>
    /// <param name="compositeKey">Key of the composite (that contains settings).</param>
    /// <param name="key">Key of the object.</param>
    /// <param name="default">Default value of the object.</param>
    /// <returns>The T object.</returns>
    public T? Read<T>(string compositeKey, string key, T? @default = default)
    {
        if (TryRead(compositeKey, out ApplicationDataCompositeValue? composite) && composite != null)
        {
            if (composite.TryGetValue(key, out var valueObj) && valueObj is string value)
            {
                return Serializer.Deserialize<T>(value);
            }
        }

        return @default;
    }

    /// <summary>
    /// Saves a group of items by its key in a composite.
    /// This method should be considered for objects that do not exceed 8k bytes during the lifetime of the application
    /// and for groups of settings which need to be treated in an atomic way.
    /// </summary>
    /// <typeparam name="T">Type of object saved.</typeparam>
    /// <param name="compositeKey">Key of the composite (that contains settings).</param>
    /// <param name="values">Objects to save.</param>
    public void Save<T>(string compositeKey, IDictionary<string, T> values)
    {
        if (TryRead(compositeKey, out ApplicationDataCompositeValue? composite) && composite != null)
        {
            foreach (KeyValuePair<string, T> setting in values)
            {
                var serializedValue = Serializer.Serialize(setting.Value) ?? string.Empty;
                if (composite.ContainsKey(setting.Key))
                {
                    composite[setting.Key] = serializedValue;
                }
                else
                {
                    composite.Add(setting.Key, serializedValue);
                }
            }
        }
        else
        {
            composite = [];
            foreach (KeyValuePair<string, T> setting in values)
            {
                var serializedValue = Serializer.Serialize(setting.Value) ?? string.Empty;
                composite.Add(setting.Key, serializedValue);
            }

            Settings.Values[compositeKey] = composite;
        }
    }

    /// <summary>
    /// Deletes a single item by its key in composite.
    /// </summary>
    /// <param name="compositeKey">Key of the composite (that contains settings).</param>
    /// <param name="key">Key of the object.</param>
    /// <returns>A boolean indicator of success.</returns>
    public bool TryDelete(string compositeKey, string key)
    {
        if (TryRead(compositeKey, out ApplicationDataCompositeValue? composite) && composite != null)
        {
            return composite.Remove(key);
        }

        return false;
    }

    /// <inheritdoc />
    public Task<T?> ReadFileAsync<T>(string filePath, T? @default = default)
    {
        return ReadFileAsync<T>(Folder, filePath, @default);
    }

    /// <inheritdoc />
    public Task<IEnumerable<(DirectoryItemType ItemType, string Name)>> ReadFolderAsync(string folderPath)
    {
        return ReadFolderAsync(Folder, folderPath);
    }

    /// <inheritdoc />
    public Task CreateFileAsync<T>(string filePath, T value)
    {
        return CreateFileAsync<T>(Folder, filePath, value);
    }

    /// <inheritdoc />
    public Task CreateFolderAsync(string folderPath)
    {
        return CreateFolderAsync(Folder, folderPath);
    }

    /// <inheritdoc />
    public Task<bool> TryDeleteItemAsync(string itemPath)
    {
        return TryDeleteItemAsync(Folder, itemPath);
    }

    /// <inheritdoc />
    public Task<bool> TryRenameItemAsync(string itemPath, string newName)
    {
        return TryRenameItemAsync(Folder, itemPath, newName);
    }

    private async Task<T?> ReadFileAsync<T>(StorageFolder folder, string filePath, T? @default = default)
    {
        string value = await StorageFileHelper.ReadTextFromFileAsync(folder, NormalizePath(filePath));
        return (value != null) ? Serializer.Deserialize<T>(value) : @default;
    }

    private async Task<IEnumerable<(DirectoryItemType, string)>> ReadFolderAsync(StorageFolder folder, string folderPath)
    {
        var targetFolder = await folder.GetFolderAsync(NormalizePath(folderPath));
        var items = await targetFolder.GetItemsAsync();

        return items.Select((item) =>
        {
            var itemType = item.IsOfType(StorageItemTypes.File) ? DirectoryItemType.File
                : item.IsOfType(StorageItemTypes.Folder) ? DirectoryItemType.Folder
                : DirectoryItemType.None;

            return (itemType, item.Name);
        });
    }

    private async Task<StorageFile> CreateFileAsync<T>(StorageFolder folder, string filePath, T value)
    {
        var serializedValue = Serializer.Serialize(value)?.ToString() ?? string.Empty;
        return await StorageFileHelper.WriteTextToFileAsync(folder, serializedValue, NormalizePath(filePath), CreationCollisionOption.ReplaceExisting);
    }

    private async Task CreateFolderAsync(StorageFolder folder, string folderPath)
    {
        await folder.CreateFolderAsync(NormalizePath(folderPath), CreationCollisionOption.OpenIfExists);
    }

    private async Task<bool> TryDeleteItemAsync(StorageFolder folder, string itemPath)
    {
        try
        {
            var item = await folder.GetItemAsync(NormalizePath(itemPath));
            await item.DeleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> TryRenameItemAsync(StorageFolder folder, string itemPath, string newName)
    {
        try
        {
            var item = await folder.GetItemAsync(NormalizePath(itemPath));
            await item.RenameAsync(newName, NameCollisionOption.FailIfExists);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string NormalizePath(string path)
    {
        var directoryName = Path.GetDirectoryName(path) ?? string.Empty;
        var fileName = Path.GetFileName(path);
        return Path.Combine(directoryName, fileName);
    }
}

#endif