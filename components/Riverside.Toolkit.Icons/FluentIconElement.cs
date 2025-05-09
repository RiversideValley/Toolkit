﻿// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Riverside.Toolkit.Icons;

/// <summary>
/// Represents an icon that uses a <see cref="FluentSymbol"/> for its content.
/// </summary>
public sealed partial class FluentIconElement : PathIcon
{
    /// <summary>
    /// Constructs an empty <see cref="FluentIconElement"/>.
    /// </summary>
    public FluentIconElement() { }

    /// <summary>
    /// Constructs a <see cref="FluentIconElement"/> displaying the specified symbol.
    /// </summary>
    /// <param name="symbol"></param>
    public FluentIconElement(FluentSymbol symbol)
    {
        this.Symbol = symbol;
    }

    /// <summary>
    /// Constructs a <see cref="FluentIconElement"/> with the specified source.
    /// </summary>
    public FluentIconElement(FluentIconSource source)
    {
        this.Data = source.Data;
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
        nameof(Symbol), typeof(FluentSymbol), typeof(FluentIconSource),
        new PropertyMetadata(null, new PropertyChangedCallback(OnSymbolChanged))
    );

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentIconElement self && (e.NewValue is FluentSymbol || e.NewValue is int))
        {
            // Set internal Data to the Path from the look-up table
            self.Data = FluentSymbolIcon.GetPathData((FluentSymbol)e.NewValue);
        }
    }
}
