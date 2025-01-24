﻿// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Riverside.Toolkit.Icons;

public partial class FluentSymbolIcon : Control
{
    private PathIcon iconDisplay;

    public FluentSymbolIcon()
    {
        this.DefaultStyleKey = typeof(FluentSymbolIcon);
    }

    /// <summary>
    /// Constructs a <see cref="FluentSymbolIcon"/> with the specified symbol.
    /// </summary>
    public FluentSymbolIcon(FluentSymbol symbol)
    {
        this.DefaultStyleKey = typeof(FluentSymbolIcon);
        this.Symbol = symbol;
    }

    /// <summary>
    /// Gets or sets the Fluent System Icons glyph used as the icon content.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Symbol"/> property.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol), typeof(FluentSymbol), typeof(FluentSymbolIcon),
        new PropertyMetadata(null, new PropertyChangedCallback(OnSymbolChanged))
    );

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("IconDisplay") is PathIcon pi)
        {
            iconDisplay = pi;
            // Awkward workaround for a weird bug where iconDisplay is null
            // when OnSymbolChanged fires in a newly created FluentSymbolIcon
            this.Symbol = this.Symbol;
        }
    }

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentSymbolIcon self && (e.NewValue is FluentSymbol || e.NewValue is int) && self.iconDisplay != null)
        {
            // Set internal Data to the Path from the look-up table
            self.iconDisplay.Data = GetPathData((FluentSymbol)e.NewValue);
        }
    }

    /// <summary>
    /// Returns a new <see cref="PathIcon"/> using the path associated with the provided <see cref="FluentSymbol"/>.
    /// </summary>
    public static PathIcon GetPathIcon(FluentSymbol symbol)
    {
        return new PathIcon
        {
            Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), GetPathData(symbol)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    /// <summary>
    /// Returns a new <see cref="Geometry"/> using the path associated with the provided <see cref="int"/>.
    /// The <paramref name="symbol"/> parameter is cast to <see cref="FluentSymbol"/>.
    /// </summary>
    public static Geometry GetPathData(int symbol) => GetPathData((FluentSymbol)symbol);

    /// <summary>
    /// Returns a new <see cref="Geometry"/> using the path associated with the provided <see cref="int"/>.
    /// </summary>
    public static Geometry GetPathData(FluentSymbol symbol)
    {
        return AllFluentIcons.TryGetValue(symbol, out string pathData)
            ? (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), pathData)
            : null;
    }
}
