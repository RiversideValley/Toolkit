// System
global using global::System;
global using global::System.Collections;
global using global::System.Collections.Generic;
global using global::System.Collections.ObjectModel;
global using global::System.Linq;
global using global::System.Threading;
global using global::System.Threading.Tasks;
global using global::System.ComponentModel;
global using global::System.Diagnostics;
global using global::System.Text.Json;
global using global::System.Text.Json.Serialization;
global using SystemIO = global::System.IO;

// WinUI
#if WinUI
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
#endif
#if UWP
global using global::Windows;
global using global::Windows.UI;
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
#endif

// CommunityToolkit

#if WinUI
global using global::CommunityToolkit.WinUI.UI.Controls;
global using global::CommunityToolkit.WinUI.UI.Helpers;
#endif

// WinUIEx

#if WinUI
global using global::WinUIEx;
#endif