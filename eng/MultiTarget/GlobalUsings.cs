﻿// System
global using global::System;
global using global::System.Collections.Generic;
global using global::System.Collections.ObjectModel;
global using global::System.ComponentModel;
global using global::System.Globalization;
global using global::System.Linq;
global using global::System.Threading.Tasks;

// WinUI
#if Uap || Uno
global using global::Windows.ApplicationModel;
global using global::Windows.ApplicationModel.Activation;
global using global::Windows.Storage;
global using global::Windows.UI;
global using global::Windows.UI.Core;
global using global::Windows.UI.WindowManagement;
global using global::Windows.System;
global using global::Windows.System.UserProfile;
global using global::Windows.System.Profile;
global using global::Windows.Storage.Search;
global using global::Windows.Storage.Streams;
#endif

#if WinUI || Uno
global using global::Microsoft;
global using global::Microsoft.UI;
global using global::Microsoft.UI.Xaml;
global using global::Microsoft.UI.Xaml.Controls;
global using global::Microsoft.UI.Xaml.Data;
global using global::Microsoft.UI.Xaml.Input;
global using global::Microsoft.UI.Xaml.Media;
global using global::Microsoft.UI.Xaml.Markup;
global using global::Microsoft.UI.Xaml.Navigation;
global using global::Microsoft.UI.Composition;
global using global::Microsoft.UI.Xaml.Documents;
global using global::Microsoft.UI.Xaml.Hosting;
global using global::Microsoft.UI.Xaml.Media.Animation;
global using global::Microsoft.UI.Xaml.Shapes;
global using global::Microsoft.UI.Xaml.Controls.Primitives;
global using global::Microsoft.UI.Xaml.Automation;
#elif Uwp
global using global::Windows;
global using global::Windows.UI.Xaml;
global using global::Windows.UI.Xaml.Controls;
global using global::Windows.UI.Xaml.Data;
global using global::Windows.UI.Xaml.Input;
global using global::Windows.UI.Xaml.Media;
global using global::Windows.UI.Xaml.Markup;
global using global::Windows.UI.Xaml.Navigation;
global using global::Windows.UI.Composition;
global using global::Windows.UI.Xaml.Documents;
global using global::Windows.UI.Xaml.Hosting;
global using global::Windows.UI.Xaml.Media.Animation;
global using global::Windows.UI.Xaml.Shapes;
global using global::Windows.UI.Xaml.Controls.Primitives;
global using global::Windows.UI.Xaml.Automation;
#elif Wpf
global using global::System.Windows;
global using global::System.Windows.Controls;
global using global::System.Windows.Data;
global using global::System.Windows.Input;
global using global::System.Windows.Media;
global using global::System.Windows.Markup;
global using global::System.Windows.Navigation;
global using global::System.Windows.Documents;
global using global::System.Windows.Media.Animation;
global using global::System.Windows.Shapes;
global using global::System.Windows.Controls.Primitives;

global using global::iNKORE.UI.WPF.Modern;
global using global::iNKORE.UI.WPF.Modern.Controls;
global using global::iNKORE.UI.WPF.Modern.Common;

global using Frame = global::iNKORE.UI.WPF.Modern.Controls.Frame;
#elif Uno
// TODO: Add Uno packages
#endif

// CommunityToolkit

#if WinUI
// TODO: Add WCT packages
#endif

// WinUIEx

#if WinAppSDK
global using global::WinUIEx;
#endif