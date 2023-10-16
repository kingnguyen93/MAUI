using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace RocketPDF.Core.Converters;

/// <summary>
/// Converts boolean to object and vice versa.
/// </summary>
public class ObjectToBoolConverter : ObjectToBoolConverter<object>
{
}

/// <summary>
/// Converts <see cref="bool"/> to object and vice versa.
/// </summary>
public class ObjectToBoolConverter<TObject> : BaseConverter<TObject, bool>
{
    /// <inheritdoc/>
    public override bool DefaultConvertReturnValue { get; set; }

    /// <inheritdoc/>
    public override TObject DefaultConvertBackReturnValue { get; set; }

    /// <summary>
    /// The object that corresponds to True value.
    /// </summary>
    public TObject TrueObject { get; set; }

    /// <summary>
    /// The object that corresponds to False value.
    /// </summary>
    public TObject FalseObject { get; set; }

    /// <summary>
    /// Converts <see cref="bool"/> to object.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">The culture to use in the converter.  This is not implemented.</param>
    /// <returns>The object assigned to <see cref="TrueObject"/> if value equals True, otherwise the value assigned to <see cref="FalseObject"/>.</returns>
    public override bool ConvertFrom(TObject value, CultureInfo culture = null)
    {
        if (default(TObject) is null && value is null && TrueObject is null)
        {
            return true;
        }

        if (value is not null)
        {
            return value.Equals(TrueObject);
        }

        ArgumentNullException.ThrowIfNull(value);

        throw new ArgumentException($"Value is not a valid {typeof(TObject).Name}", nameof(value));
    }

    /// <summary>
    /// Converts back object to <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="culture">The culture to use in the converter.  This is not implemented.</param>
    /// <returns>True if value equals <see cref="TrueObject"/>, otherwise False.</returns>
    public override TObject ConvertBackTo(bool value, CultureInfo culture = null) => value ? TrueObject : FalseObject;
}