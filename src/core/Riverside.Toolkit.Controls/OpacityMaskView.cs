// Totally not stolen from https://github.com/cnbluefire

#if WINDOWS10_0_18362_0_OR_GREATER

using System.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace Riverside.Toolkit.Controls;

/// <summary>
/// A custom control that provides an opacity mask view.
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "A compiler #if block was used to disable this call site on versions prior to 10.0.18362.0 so this warning is unnecessary.")]
public partial class OpacityMaskView : RedirectVisualView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpacityMaskView"/> class.
    /// </summary>
    public OpacityMaskView()
    {
        opacityMaskHost = new Rectangle();

        compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        opacityMaskVisualSurface = compositor.CreateVisualSurface();
        opacityMaskVisualBrush = compositor.CreateSurfaceBrush(opacityMaskVisualSurface);
        opacityMaskVisualBrush.HorizontalAlignmentRatio = 0;
        opacityMaskVisualBrush.VerticalAlignmentRatio = 0;
        opacityMaskVisualBrush.Stretch = CompositionStretch.None;

        maskBrush = compositor.CreateMaskBrush();
        maskBrush.Mask = opacityMaskVisualBrush;
        maskBrush.Source = this.ChildVisualBrush;

        this.RootVisual.Brush = maskBrush;
    }

    private readonly Rectangle opacityMaskHost;

    private readonly Compositor compositor;

    private readonly CompositionVisualSurface opacityMaskVisualSurface;
    private readonly CompositionSurfaceBrush opacityMaskVisualBrush;
    private readonly CompositionMaskBrush maskBrush;

    /// <summary>
    /// Gets or sets the opacity mask brush.
    /// </summary>
    public Brush OpacityMask
    {
        get => (Brush)GetValue(OpacityMaskProperty);
        set => SetValue(OpacityMaskProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="OpacityMask"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OpacityMaskProperty =
        DependencyProperty.Register("OpacityMask", typeof(Brush), typeof(OpacityMaskView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), OnOpacityMaskPropertyChanged));

    /// <summary>
    /// Called when the <see cref="OpacityMask"/> property changes.
    /// </summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The event data.</param>
    private static void OnOpacityMaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OpacityMaskView sender && !Equals(e.NewValue, e.OldValue))
        {
            sender.opacityMaskHost.Fill = sender.RedirectVisualAttached ? e.NewValue as Brush : null;
        }
    }

    /// <summary>
    /// Detaches the visuals from the control.
    /// </summary>
    protected override void OnDetachVisuals()
    {
        base.OnDetachVisuals();

        if (this.LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Fill = null;
            }

            opacityMaskVisualSurface.SourceVisual = null;

            if (this.OpacityMaskContainer != null)
            {
                this.OpacityMaskContainer.Visibility = Visibility.Collapsed;
                _ = this.OpacityMaskContainer.Children.Remove(opacityMaskHost);
            }
        }
    }

    /// <summary>
    /// Attaches the visuals to the control.
    /// </summary>
    protected override void OnAttachVisuals()
    {
        base.OnAttachVisuals();

        if (this.LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Fill = this.OpacityMask;
                opacityMaskVisualSurface.SourceVisual = ElementCompositionPreview.GetElementVisual(opacityMaskHost);
            }

            if (this.OpacityMaskContainer != null)
            {
                this.OpacityMaskContainer.Visibility = Visibility.Visible;
                ElementCompositionPreview.GetElementVisual(this.OpacityMaskContainer).IsVisible = false;
                this.OpacityMaskContainer.Children.Add(opacityMaskHost);
            }
        }
    }

    /// <summary>
    /// Updates the size of the control.
    /// </summary>
    protected override void OnUpdateSize()
    {
        base.OnUpdateSize();

        if (this.RedirectVisualAttached && this.LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Width = this.LayoutRoot.ActualWidth;
                opacityMaskHost.Height = this.LayoutRoot.ActualHeight;
            }

            opacityMaskVisualSurface.SourceSize = new Vector2((float)this.LayoutRoot.ActualWidth, (float)this.LayoutRoot.ActualHeight);
        }
    }
}
#endif