using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RocketPDF.Core.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool IsBase64String(this string base64EncodedData)
        {
            Span<byte> buffer = new(new byte[base64EncodedData.Length]);
            return Convert.TryFromBase64String(base64EncodedData, buffer, out _);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string RemoveDuplicateSpaces(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return Regex.Replace(input, @"\s+", " ");
        }

        public static string RemoveSpecialCharacters(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return Regex.Replace(input.Replace("[", "").Replace("]", ""), "/[:!@#$%^&*()}{|\":?><\\;'/.,~]/g", "");
        }

        public static string CleanString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.RemoveDuplicateSpaces().RemoveSpecialCharacters();
        }

        public static string ToTitleCase(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }

        public static string ToCamelCase(this string str)
        {
            return string.Concat(str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToTitleCase()));
        }

        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((letter, index) => index > 0 && char.IsUpper(letter) ? "_" + letter : letter.ToString())).ToLower();
        }
    }
}