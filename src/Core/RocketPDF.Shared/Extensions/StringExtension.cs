using System.Text;
using System.Text.RegularExpressions;

namespace RocketPDF.Shared.Extensions
{
    public static partial class StringExtension
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

        public static short ToShort(this string str)
        {
            return Convert.ToInt16(str);
        }

        public static int ToInt(this string str)
        {
            return Convert.ToInt32(str);
        }

        public static long ToLong(this string str)
        {
            return Convert.ToInt64(str);
        }

        public static decimal ToDecimal(this string str)
        {
            return Convert.ToDecimal(str);
        }

        public static float ToFloat(this string str)
        {
            return Convert.ToSingle(str);
        }

        public static double ToDouble(this string str)
        {
            return Convert.ToDouble(str);
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

        [GeneratedRegex("\\s+")]
        private static partial Regex DuplicateSpacesRegex();

        public static string RemoveDuplicateSpaces(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return DuplicateSpacesRegex().Replace(input, " ");
        }

        [GeneratedRegex("/[:!@#$%^&*()}{|\":?><\\;'/.,~]/g")]
        private static partial Regex SpecialCharactersRegex();

        public static string RemoveSpecialCharacters(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return SpecialCharactersRegex().Replace(input.Replace("[", "").Replace("]", ""), "");
        }

        public static string CleanString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.RemoveDuplicateSpaces().RemoveSpecialCharacters();
        }

        public static string ToCamelCase(this string str)
        {
            return string.Concat(str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x[0].ToString().ToUpper() + x[1..]));
        }

        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((letter, index) => index > 0 && char.IsUpper(letter) ? "_" + letter : letter.ToString())).ToLower();
        }

        public static string ToSnakeCaseBySpan(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return string.Empty;

            int upperCaseLength = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= 'A' && str[i] <= 'Z' && str[i] != str[0])
                {
                    upperCaseLength++;
                }
            }

            int bufferSize = str.Length + upperCaseLength;
            Span<char> buffer = new char[bufferSize];
            int bufferPosition = 0;
            int namePosition = 0;

            while (bufferPosition < buffer.Length)
            {
                if (namePosition > 0 && str[namePosition] >= 'A' && str[namePosition] <= 'Z')
                {
                    buffer[bufferPosition] = '_';
                    buffer[bufferPosition + 1] = char.ToLowerInvariant(str[namePosition]);
                    bufferPosition += 2;
                    namePosition++;
                    continue;
                }
                buffer[bufferPosition] = char.ToLowerInvariant(str[namePosition]);
                bufferPosition++;
                namePosition++;
            }
            return buffer.ToString();
        }

        public static string Join(this string[] input, string separator)
        {
            return string.Join(separator, input);
        }

        public static string[] SplitOrDefault(this string str, char separator)
        {
            return str?.Split(separator).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray() ?? Array.Empty<string>();
        }
    }
}