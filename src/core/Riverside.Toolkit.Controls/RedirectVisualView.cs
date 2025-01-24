﻿// Totally not stolen from https://github.com/cnbluefire

using System.Numerics;

namespace Riverside.Toolkit.Controls;

/// <summary>
/// A custom control that redirects visual content.
/// </summary>
[ContentProperty(Name = nameof(Child))]
public partial class RedirectVisualView : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectVisualView"/> class.
    /// </summary>
    public RedirectVisualView()
    {
        this.DefaultStyleKey = typeof(RedirectVisualView);

        childVisualBrushOffsetEnabled = this.ChildVisualBrushOffsetEnabled;

        hostVisual = ElementCompositionPreview.GetElementVisual(this);
        compositor = hostVisual.Compositor;

        childVisualSurface = compositor.CreateVisualSurface();
        childVisualBrush = compositor.CreateSurfaceBrush(childVisualSurface);
        childVisualBrush.HorizontalAlignmentRatio = 0;
        childVisualBrush.VerticalAlignmentRatio = 0;
        childVisualBrush.Stretch = CompositionStretch.None;

        redirectVisual = compositor.CreateSpriteVisual();
        redirectVisual.RelativeSizeAdjustment = Vector2.One;
        redirectVisual.Brush = childVisualBrush;

        if (childVisualBrushOffsetEnabled)
        {
            offsetBind = compositor.CreateExpressionAnimation("Vector2(visual.Offset.X, visual.Offset.Y)");
        }

        Loaded += RedirectVisualView_Loaded;
        Unloaded += RedirectVisualView_Unloaded;
        _ = RegisterPropertyChangedCallback(PaddingProperty, OnPaddingPropertyChanged);
    }

    /// <summary>
    /// Gets a value indicating whether the child visual brush offset is enabled.
    /// </summary>
    protected virtual bool ChildVisualBrushOffsetEnabled => true;

    private bool measureChildInBoundingBox = true;

    /// <summary>
    /// Gets or sets a value indicating whether to measure the child in the bounding box.
    /// </summary>
    protected bool MeasureChildInBoundingBox
    {
        get => measureChildInBoundingBox;
        set
        {
            if (measureChildInBoundingBox != value)
            {
                measureChildInBoundingBox = value;
                UpdateMeasureChildInBoundingBox();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the redirect visual is attached.
    /// </summary>
    protected bool RedirectVisualAttached => attached;

    /// <summary>
    /// Gets or sets a value indicating whether the redirect visual is enabled.
    /// </summary>
    protected bool RedirectVisualEnabled
    {
        get => redirectVisualEnabled;
        set
        {
            if (redirectVisualEnabled != value)
            {
                redirectVisualEnabled = value;

                if (value)
                {
                    if (this.IsLoaded)
                    {
                        AttachVisuals();
                    }
                }
                else
                {
                    DetachVisuals();
                }
            }
        }
    }

    private bool attached;
    private bool redirectVisualEnabled = true;
    private readonly bool childVisualBrushOffsetEnabled;

    private Grid layoutRoot;
    private ContentPresenter childPresenter;
    private Grid childPresenterContainer;
    private Canvas ChildHost;
    private Canvas opacityMaskContainer;

    /// <summary>
    /// Gets or sets the layout root grid.
    /// </summary>
    protected Grid LayoutRoot
    {
        get => layoutRoot;
        private set
        {
            if (layoutRoot != value)
            {
                Grid old = layoutRoot;

                layoutRoot = value;

                if (old != null)
                {
                    old.SizeChanged -= LayoutRoot_SizeChanged;
                }

                if (layoutRoot != null)
                {
                    layoutRoot.SizeChanged += LayoutRoot_SizeChanged;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the child presenter.
    /// </summary>
    protected ContentPresenter ChildPresenter
    {
        get => childPresenter;
        private set
        {
            if (childPresenter != value)
            {
                ContentPresenter old = childPresenter;

                childPresenter = value;

                if (old != null)
                {
                    old.SizeChanged -= ChildPresenter_SizeChanged;
                }

                if (childPresenter != null)
                {
                    childPresenter.SizeChanged += ChildPresenter_SizeChanged;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the child presenter container.
    /// </summary>
    protected Grid ChildPresenterContainer
    {
        get => childPresenterContainer;
        private set
        {
            if (childPresenterContainer != value)
            {
                childPresenterContainer = value;

                UpdateMeasureChildInBoundingBox();
            }
        }
    }

    /// <summary>
    /// Gets or sets the opacity mask container.
    /// </summary>
    protected Canvas OpacityMaskContainer
    {
        get => opacityMaskContainer;
        private set => opacityMaskContainer = value;
    }

    private readonly Visual hostVisual;
    private readonly Compositor compositor;

    private readonly CompositionVisualSurface childVisualSurface;
    private readonly CompositionSurfaceBrush childVisualBrush;

    private SpriteVisual redirectVisual;
    private readonly ExpressionAnimation offsetBind;

    /// <summary>
    /// Gets the child visual brush.
    /// </summary>
    protected CompositionBrush ChildVisualBrush => childVisualBrush;

    /// <summary>
    /// Gets or sets the root visual.
    /// </summary>
    protected SpriteVisual RootVisual
    {
        get => redirectVisual;
        set => redirectVisual = value;
    }

    /// <summary>
    /// Applies the control template and attaches visuals if enabled.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DetachVisuals();

        this.LayoutRoot = GetTemplateChild(nameof(this.LayoutRoot)) as Grid;
        this.ChildPresenter = GetTemplateChild(nameof(this.ChildPresenter)) as ContentPresenter;
        this.ChildPresenterContainer = GetTemplateChild(nameof(this.ChildPresenterContainer)) as Grid;
        ChildHost = GetTemplateChild(nameof(ChildHost)) as Canvas;
        this.OpacityMaskContainer = GetTemplateChild(nameof(this.OpacityMaskContainer)) as Canvas;

        if (this.RedirectVisualEnabled)
        {
            AttachVisuals();
        }
    }

    /// <summary>
    /// Gets or sets the child element.
    /// </summary>
    public UIElement Child
    {
        get => (UIElement)GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Child"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChildProperty =
        DependencyProperty.Register("Child", typeof(UIElement), typeof(RedirectVisualView), new PropertyMetadata(null));

    /// <summary>
    /// Attaches the visuals to the control.
    /// </summary>
    private void AttachVisuals()
    {
        if (attached) return;

        attached = true;

        if (this.LayoutRoot != null)
        {
            if (this.ChildPresenter != null)
            {
                Visual childBorderVisual = ElementCompositionPreview.GetElementVisual(this.ChildPresenter);

                childVisualSurface.SourceVisual = childBorderVisual;

                if (offsetBind != null)
                {
                    offsetBind.SetReferenceParameter("visual", childBorderVisual);
                    childVisualBrush.StartAnimation("Offset", offsetBind);
                }
            }

            if (this.ChildPresenterContainer != null)
            {
                ElementCompositionPreview.GetElementVisual(this.ChildPresenterContainer).IsVisible = false;
            }

            if (this.OpacityMaskContainer != null)
            {
                ElementCompositionPreview.GetElementVisual(this.OpacityMaskContainer).IsVisible = false;
            }

            if (ChildHost != null)
            {
                ElementCompositionPreview.SetElementChildVisual(ChildHost, redirectVisual);
            }

            UpdateSize();
        }

        OnAttachVisuals();
    }

    /// <summary>
    /// Detaches the visuals from the control.
    /// </summary>
    private void DetachVisuals()
    {
        if (!attached) return;

        attached = false;

        if (this.LayoutRoot != null)
        {
            childVisualSurface.SourceVisual = null;

            if (offsetBind != null)
            {
                childVisualBrush.StopAnimation("Offset");
                offsetBind.ClearAllParameters();
            }

            if (this.ChildPresenterContainer != null)
            {
                ElementCompositionPreview.GetElementVisual(this.ChildPresenterContainer).IsVisible = true;
            }

            if (this.OpacityMaskContainer != null)
            {
                ElementCompositionPreview.GetElementVisual(this.OpacityMaskContainer).IsVisible = true;
            }

            if (ChildHost != null)
            {
                ElementCompositionPreview.SetElementChildVisual(ChildHost, null);
            }
        }

        OnDetachVisuals();
    }

    /// <summary>
    /// Handles the Unloaded event of the control.
    /// </summary>
    private void RedirectVisualView_Unloaded(object sender, RoutedEventArgs e) => DetachVisuals();

    /// <summary>
    /// Handles the Loaded event of the control.
    /// </summary>
    private void RedirectVisualView_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.RedirectVisualEnabled)
        {
            AttachVisuals();
        }
    }

    /// <summary>
    /// Handles the Padding property changed event.
    /// </summary>
    private void OnPaddingPropertyChanged(DependencyObject sender, DependencyProperty dp) => UpdateSize();

    /// <summary>
    /// Handles the SizeChanged event of the layout root.
    /// </summary>
    private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateSize();

    /// <summary>
    /// Handles the SizeChanged event of the child presenter.
    /// </summary>
    private void ChildPresenter_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateSize();

    /// <summary>
    /// Updates the size of the control.
    /// </summary>
    private void UpdateSize()
    {
        if (attached && this.LayoutRoot != null)
        {
            if (this.ChildPresenter != null)
            {
                childVisualSurface.SourceSize = new Vector2((float)this.ChildPresenter.ActualWidth, (float)this.ChildPresenter.ActualHeight);
            }
        }

        OnUpdateSize();
    }

    /// <summary>
    /// Updates the measure child in bounding box property.
    /// </summary>
    private void UpdateMeasureChildInBoundingBox()
    {
        if (this.ChildPresenterContainer != null)
        {
            bool value = this.MeasureChildInBoundingBox;

            var length = new GridLength(1, value ? GridUnitType.Star : GridUnitType.Auto);

            if (this.ChildPresenterContainer.RowDefinitions.Count > 0)
            {
                this.ChildPresenterContainer.RowDefinitions[0].Height = length;
            }
            if (this.ChildPresenterContainer.ColumnDefinitions.Count > 0)
            {
                this.ChildPresenterContainer.ColumnDefinitions[0].Width = length;
            }
        }
    }

    /// <summary>
    /// Called when visuals are attached.
    /// </summary>
    protected virtual void OnAttachVisuals()
    {

    }

    /// <summary>
    /// Called when visuals are detached.
    /// </summary>
    protected virtual void OnDetachVisuals()
    {

    }

    /// <summary>
    /// Called when the size is updated.
    /// </summary>
    protected virtual void OnUpdateSize()
    {

    }
}
