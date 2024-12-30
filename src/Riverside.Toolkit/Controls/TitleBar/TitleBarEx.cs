using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUIEx;
using WinUIEx.Messaging;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
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
            DefaultStyleKey = typeof(TitleBarEx);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Using GetTemplateChild<T> to safely retrieve template children and cast them
            CloseButton = GetTemplateChild<Button>("CloseButton");
            MaximizeRestoreButton = GetTemplateChild<ToggleButton>("MaximizeButton");
            MinimizeButton = GetTemplateChild<Button>("MinimizeButton");
            TitleTextBlock = GetTemplateChild<TextBlock>("TitleTextBlock");
            TitleBarIcon = GetTemplateChild<Image>("TitleBarIcon");
            AccentStrip = GetTemplateChild<Border>("AccentStrip");
            CustomRightClickFlyout = GetTemplateChild<MenuFlyout>("CustomRightClickFlyout");

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
            CurrentWindow = windowEx;

            // Configure title bar
            CurrentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            CurrentWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;

            // Attach pointer events
            var content = CurrentWindow.Content;
            content.PointerMoved += CheckMouseButtonDownPointerEvent;
            content.PointerReleased += CheckMouseButtonDownPointerEvent;
            content.PointerExited += CheckMouseButtonDownPointerEvent;

            content.PointerEntered += SwitchButtonStatePointerEvent;
            PointerExited += SwitchButtonStatePointerEvent;

            // Attach window events
            CurrentWindow.WindowStateChanged += CurrentWindow_WindowStateChanged;
            CurrentWindow.Closed += CurrentWindow_Closed;

            // Initialize window properties and behaviors
            UpdateWindowSizeAndPosition();
            UpdateWindowProperties();
            AttachWndProc();
            LoadDragRegion();
        }
    }
}