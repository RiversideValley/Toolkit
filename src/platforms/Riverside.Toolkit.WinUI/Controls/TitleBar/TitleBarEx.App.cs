﻿#if WinUI
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage;
using System;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar;

public partial class TitleBarEx
{
    private static T? GetValue<T>(string key)
    {
        try
        {
            var userSettings = ApplicationData.GetDefault();
            return (T)userSettings.LocalSettings.Values[key];
        }
        catch
        {
            return default;
        }
    }

    private static void SetValue<T>(string key, T newValue)
    {
        try
        {
            var userSettings = ApplicationData.GetDefault();
            userSettings.LocalSettings.Values[key] = newValue;
        }
        catch
        {
            return;
        }
    }

    private T GetTemplateChild<T>(string name) where T : DependencyObject
    {
        DependencyObject child = GetTemplateChild(name);
        return child is T typedChild
            ? typedChild
            : throw new InvalidOperationException($"The template child '{name}' is not of type {typeof(T).FullName}.");
    }
}
#endif