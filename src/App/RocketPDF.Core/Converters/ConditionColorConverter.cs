using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Checks whether the incoming value equals the provided parameter.
/// </summary>
public class ConditionColorConverter : BaseConverterOneWay<object, Color, object>
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
    /// Checks whether the incoming value doesn't equal the provided parameter.
    /// </summary>
    /// <param name="value">The first object to compare.</param>
    /// <param name="parameter">The second object to compare.</param>
    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
    /// <returns>True if <paramref name="value"/> and <paramref name="parameter"/> are equal, False if they are not equal.</returns>
    public override Color ConvertFrom(object value, object parameter, CultureInfo culture = null)
    {
        if (value is null)
        {
            return FalseColor;
        }
        return value.Equals(parameter) ? TrueColor : FalseColor;
    }
}