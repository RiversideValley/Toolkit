#if WinUI
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUIEx;
using WinUIEx.Messaging;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar;

public partial class TitleBarEx : Control
{
    // The window
    protected WindowEx? CurrentWindow { get; private set; }

    // UI controls
    protected Button? CloseButton { get; private set; }
    protected ToggleButton? MaximizeRestoreButton { get; private set; }
    protected Button? MinimizeButton { get; private set; }
    protected TextBlock? TitleTextBlock { get; private set; }
    protected Image? TitleBarIcon { get; private set; }
    protected Border? AccentStrip { get; private set; }
    protected MenuFlyout? CustomRightClickFlyout { get; private set; }

    // Local variables
    protected SelectedCaptionButton CurrentCaption { get; private set; } = SelectedCaptionButton.None;
    private WindowMessageMonitor? messageMonitor;
    private bool isWindowFocused = false;
    private bool isMaximized = false;
    private int buttonDownHeight = 0;
    private double additionalHeight = 0;
    private bool closed = false;
    private bool allowSizeCheck = false;

    public TitleBarEx()
    {
        this.DefaultStyleKey = typeof(TitleBarEx);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Using GetTemplateChild<T> to safely retrieve template children and cast them
        this.CloseButton = GetTemplateChild<Button>("CloseButton");
        this.MaximizeRestoreButton = GetTemplateChild<ToggleButton>("MaximizeButton");
        this.MinimizeButton = GetTemplateChild<Button>("MinimizeButton");
        this.TitleTextBlock = GetTemplateChild<TextBlock>("TitleTextBlock");
        this.TitleBarIcon = GetTemplateChild<Image>("TitleBarIcon");
        this.AccentStrip = GetTemplateChild<Border>("AccentStrip");
        this.CustomRightClickFlyout = GetTemplateChild<MenuFlyout>("CustomRightClickFlyout");

        // Using GetTemplateChild<T> to safely retrieve template children and cast them, then subscribe to events
        GetTemplateChild<MenuFlyoutItem>("MaximizeContextMenuItem").Click += MaximizeContextMenu_Click;
        GetTemplateChild<MenuFlyoutItem>("SizeContextMenuItem").Click += SizeContextMenu_Click;
        GetTemplateChild<MenuFlyoutItem>("MoveContextMenuItem").Click += MoveContextMenu_Click;
        GetTemplateChild<MenuFlyoutItem>("MinimizeContextMenuItem").Click += MinimizeContextMenu_Click;
        GetTemplateChild<MenuFlyoutItem>("CloseContextMenuItem").Click += CloseContextMenu_Click;
        GetTemplateChild<MenuFlyoutItem>("RestoreContextMenuItem").Click += RestoreContextMenu_Click;
    }

    public void InitializeForWindow(WindowEx windowEx, Application app)
    {
        this.CurrentWindow = windowEx;

        // Configure title bar
        this.CurrentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        this.CurrentWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;

        // Attach pointer events
        var content = (FrameworkElement)this.CurrentWindow.Content;
        content.PointerMoved += CheckMouseButtonDownPointerEvent;
        content.PointerReleased += CheckMouseButtonDownPointerEvent;
        content.PointerExited += CheckMouseButtonDownPointerEvent;
        content.PointerEntered += SwitchButtonStatePointerEvent;
        PointerExited += SwitchButtonStatePointerEvent;

        // Attach window events
        this.CurrentWindow.WindowStateChanged += CurrentWindow_WindowStateChanged;
        this.CurrentWindow.Closed += CurrentWindow_Closed;
        this.CurrentWindow.SizeChanged += CurrentWindow_SizeChanged;
        this.CurrentWindow.PositionChanged += CurrentWindow_PositionChanged;

        // Attach load events
        content.Loaded += ContentLoaded;

        // Initialize window properties and behaviors
        UpdateWindowSizeAndPosition();
        UpdateWindowProperties();
        AttachWndProc();
        LoadDragRegion();
        InvokeChecks();
    }
}
#endif