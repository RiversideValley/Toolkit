﻿using Riverside.Extensions.Accountability;
using Riverside.Toolkit.Helpers; // This is being used instead of the Windows Community Toolkit version as using WCT here causes some issues.
using System.Numerics;

/* Not adding an entire new dependency just for a fancy XML comment.
#if !WINDOWS10_0_19041_0
using CommunityToolkit.WinUI;
#endif */

namespace Riverside.Toolkit.Controls;

// TODO: Add XAML resource dictionary back when .NET SDK error is fixed.

/// <summary>
/// The <see cref="DropShadowPanel"/> control allows the creation of a DropShadow for any XAML <see cref="FrameworkElement"/> in markup
/// making it easier to add shadows to XAML without having to directly drop down to composition APIs.
/// </summary>
/// <remarks>
/// This control was originally created by the Windows Community Toolkit and has been modified to work with the latest version of the Windows UI Library.
/// It is available in CubeKit for reference purposes (including use in GlowUI controls), please use <c>AttachedDropShadow</c> or <c>AttachedCardShadow</c> instead.
/// </remarks>
[NotMyCode("The .NET Foundation licenses this file to you under the MIT license.", "MIT")]
[Obsolete("Please use the AttachedDropShadow or AttachedCardShadow implementations instead.")]
[TemplatePart(Name = PartShadow, Type = typeof(Border))]
public partial class DropShadowPanel : ContentControl
{
    private const string PartShadow = "ShadowElement";

    private readonly DropShadow _dropShadow;
    private readonly SpriteVisual _shadowVisual;
    private Border _border;

    /// <summary>
    /// Initializes a new instance of the <see cref="DropShadowPanel"/> class.
    /// </summary>
    public DropShadowPanel()
    {
        this.DefaultStyleKey = typeof(DropShadowPanel);

        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _shadowVisual = compositor.CreateSpriteVisual();

            _dropShadow = compositor.CreateDropShadow();
            _shadowVisual.Shadow = _dropShadow;
        }
    }

    /// <summary>
    /// Update the visual state of the control when its template is changed.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        if (DesignTimeHelpers.IsRunningInLegacyDesignerMode)
        {
            return;
        }

        _border = GetTemplateChild(PartShadow) as Border;

        if (_border != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_border, _shadowVisual);
        }

        ConfigureShadowVisualForCastingElement();

        base.OnApplyTemplate();
    }

    /// <inheritdoc/>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        if (oldContent != null)
        {
            if (oldContent is FrameworkElement oldElement)
            {
                oldElement.SizeChanged -= OnSizeChanged;
            }
        }

        if (newContent != null)
        {
            if (newContent is FrameworkElement newElement)
            {
                newElement.SizeChanged += OnSizeChanged;
            }
        }

        UpdateShadowMask();

        base.OnContentChanged(oldContent, newContent);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
        {
            UpdateShadowSize();
        }
    }

    private void ConfigureShadowVisualForCastingElement()
    {
        UpdateShadowMask();

        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
        {
            UpdateShadowSize();
        }
    }

    private void OnBlurRadiusChanged(double newValue)
    {
        if (_dropShadow != null)
        {
            _dropShadow.BlurRadius = (float)newValue;
        }
    }

    private void OnColorChanged(Color newValue)
    {
        if (_dropShadow != null)
        {
            _dropShadow.Color = newValue;
        }
    }

    private void OnOffsetXChanged(double newValue)
    {
        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode && _dropShadow != null)
        {
            UpdateShadowOffset((float)newValue, _dropShadow.Offset.Y, _dropShadow.Offset.Z);
        }
    }

    private void OnOffsetYChanged(double newValue)
    {
        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode && _dropShadow != null)
        {
            UpdateShadowOffset(_dropShadow.Offset.X, (float)newValue, _dropShadow.Offset.Z);
        }
    }

    private void OnOffsetZChanged(double newValue)
    {
        if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode && _dropShadow != null)
        {
            UpdateShadowOffset(_dropShadow.Offset.X, _dropShadow.Offset.Y, (float)newValue);
        }
    }

    private void OnShadowOpacityChanged(double newValue)
    {
        if (_dropShadow != null)
        {
            _dropShadow.Opacity = (float)newValue;
        }
    }

    private void UpdateShadowMask()
    {
        if (DesignTimeHelpers.IsRunningInLegacyDesignerMode)
        {
            return;
        }

        if (Content != null && IsMasked)
        {
            CompositionBrush mask = null;

            // We check for IAlphaMaskProvider first, to ensure that we use the custom
            // alpha mask even if Content happens to extend any of the other classes
            if (Content is IAlphaMaskProvider maskedControl)
            {
                if (maskedControl.WaitUntilLoaded && maskedControl is FrameworkElement element && !element.IsLoaded)
                {
                    element.Loaded += CustomMaskedElement_Loaded;
                }
                else
                {
                    mask = maskedControl.GetAlphaMask();
                }
            }
            else if (Content is Image)
            {
                mask = ((Image)Content).GetAlphaMask();
            }
            else if (Content is Shape)
            {
                mask = ((Shape)Content).GetAlphaMask();
            }
            else if (Content is TextBlock)
            {
                mask = ((TextBlock)Content).GetAlphaMask();
            }

            _dropShadow.Mask = mask;
        }
        else
        {
            _dropShadow.Mask = null;
        }
    }

    private void CustomMaskedElement_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= CustomMaskedElement_Loaded;

            _dropShadow.Mask = ((IAlphaMaskProvider)element).GetAlphaMask();
        }
    }

    private void UpdateShadowOffset(float x, float y, float z)
    {
        if (_dropShadow != null)
        {
            _dropShadow.Offset = new Vector3(x, y, z);
        }
    }

    private void UpdateShadowSize()
    {
        if (_shadowVisual != null)
        {
            Vector2 newSize = new Vector2(0, 0);
            if (Content is FrameworkElement contentFE)
            {
                newSize = new Vector2((float)contentFE.ActualWidth, (float)contentFE.ActualHeight);
            }

            _shadowVisual.Size = newSize;
        }
    }
}