using CommunityToolkit.Maui.Converters;
using System.Collections;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Converts an <see cref="short"/> to a corresponding <see cref="bool"/> and vice versa.
/// </summary>
public class HasValueConverter : BaseConverter<object, bool>
{
    /// <inheritdoc/>
    public override bool DefaultConvertReturnValue { get; set; } = false;

    /// <inheritdoc />
    public override object DefaultConvertBackReturnValue { get; set; }

    /// <summary>
    /// Converts the incoming <see cref="short"/> to a <see cref="bool"/> indicating whether or not the value is not equal to 0.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">(Not Used)</param>
    /// <returns><c>false</c> if the supplied <paramref name="value"/> is equal to <c>0</c> and <c>true</c> otherwise.</returns>
    public override bool ConvertFrom(object value, CultureInfo culture = null)
    {
        if (value == null)
            return false;
        if (value is string)
            return !string.IsNullOrWhiteSpace((string)value);
        if (value is IList)
            return ((IList)value).Count > 0;
        return value != default;
    }

    /// <summary>
    /// Converts the incoming <see cref="bool"/> to an <see cref="short"/> indicating whether or not the value is true.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">(Not Used)</param>
    /// <returns><c>1</c> if the supplied <paramref name="value"/> is <c>true</c> and <c>0</c> otherwise.</returns>
    public override object ConvertBackTo(bool value, CultureInfo culture = null) => default;
}