using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Converts an <see cref="short"/> to a corresponding <see cref="bool"/> and vice versa.
/// </summary>
public class StringToIntConverter : BaseConverter<string, int>
{
    /// <inheritdoc/>
    public override int DefaultConvertReturnValue { get; set; }

    /// <inheritdoc />
    public override string DefaultConvertBackReturnValue { get; set; }

    /// <summary>
    /// Converts the incoming <see cref="short"/> to a <see cref="bool"/> indicating whether or not the value is not equal to 0.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">(Not Used)</param>
    /// <returns><c>false</c> if the supplied <paramref name="value"/> is equal to <c>0</c> and <c>true</c> otherwise.</returns>
    public override int ConvertFrom(string value, CultureInfo culture = null) => Convert.ToInt32(value);

    /// <summary>
    /// Converts the incoming <see cref="bool"/> to an <see cref="short"/> indicating whether or not the value is true.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">(Not Used)</param>
    /// <returns><c>1</c> if the supplied <paramref name="value"/> is <c>true</c> and <c>0</c> otherwise.</returns>
    public override string ConvertBackTo(int value, CultureInfo culture = null) => value.ToString();
}