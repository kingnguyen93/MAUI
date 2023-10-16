using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Checks whether the incoming value equals the provided parameter.
/// </summary>
public class IntToTextConverter : BaseConverterOneWay<object, string, object>
{
    /// <inheritdoc/>
    public override string DefaultConvertReturnValue { get; set; }

    /// <summary>
    /// The object that corresponds to True value.
    /// </summary>
    public string TrueText { get; set; }

    /// <summary>
    /// The object that corresponds to False value.
    /// </summary>
    public string FalseText { get; set; }

    /// <summary>
    /// Checks whether the incoming value doesn't equal the provided parameter.
    /// </summary>
    /// <param name="value">The first object to compare.</param>
    /// <param name="parameter">The second object to compare.</param>
    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
    /// <returns>True if <paramref name="value"/> and <paramref name="parameter"/> are equal, False if they are not equal.</returns>
    public override string ConvertFrom(object value, object parameter, CultureInfo culture = null)
        => Convert.ToInt32(value) == Convert.ToInt32(parameter) ? TrueText : FalseText;
}