#if !Uwp

namespace Riverside.Extensions.WinUI;

public abstract partial class WindowEx
#if WinAppSDK
    : WinUIEx.WindowEx
#elif Uno || Wpf
    : Window
#endif
{ 
    public WindowEx()
    {
        
    }
#if !WinAppSDK // Polyfill for Uno/WPF
    public double Height
    {
        get => this.AppWindow.Size.Height;
        set => this.AppWindow.Size.Height = value;
    }
#endif
}

#endif
