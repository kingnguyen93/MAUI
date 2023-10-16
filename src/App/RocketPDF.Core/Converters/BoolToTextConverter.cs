using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Checks whether the incoming value equals the provided parameter.
/// </summary>
public class BoolToTextConverter : BaseConverterOneWay<bool, string>
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
    /// .
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
    public override string ConvertFrom(bool value, CultureInfo culture = null)
    {
        return value ? TrueText : FalseText;
    }
}