using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Checks whether the incoming value equals the provided parameter.
/// </summary>
public class BoolToColorConverter : BaseConverterOneWay<bool, Color>
{
    /// <inheritdoc/>
    public override Color DefaultConvertReturnValue { get; set; }

    /// <summary>
    /// The object that corresponds to True value.
    /// </summary>
    public Color TrueColor { get; set; }

    /// <summary>
    /// The object that corresponds to False value.
    /// </summary>
    public Color FalseColor { get; set; }

    /// <summary>
    /// .
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
    public override Color ConvertFrom(bool value, CultureInfo culture = null)
    {
        return value ? TrueColor : FalseColor;
    }
}