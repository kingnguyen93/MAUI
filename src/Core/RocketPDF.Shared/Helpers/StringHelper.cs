using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RocketPDF.Shared.Helpers
{
    public static class StringHelper
    {
        private const string PhoneNumbers = "090,091,092,093,094,095,096,097,098,099,039,038,037,036,035,034,033,032,070,079,078,077,076,075,08,081,082,083,084,085,052,056,058,059,055,00";
        private const string InternationalPhoneNumbers = "8490,8491,8492,8493,8494,8495,8496,8497,8498,8499,8439,8438,8437,8436,8435,8434,8433,8432,8470,8479,8478,8477,8476,8475,848,8481,8482,8483,8484,8485,8452,8456,8458,8459,840";

        public static bool IsValidPhoneNumber(this string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return true;

                string phoneNumber = Regex.Replace(input, @"\s+", "").Trim();

                if (phoneNumber.Length > 15 || phoneNumber.Length < 10)
                    return false;

                //if ((phoneNumber.StartsWith("00") && phoneNumber.Length > 15) || phoneNumber.Length > 10 || phoneNumber.Length < 10)
                //    return false;

                if (!Regex.Match(input, "^[0-9-]*$").Success)
                    return false;

                foreach (var item in PhoneNumbers.Split(","))
                {
                    if (phoneNumber.StartsWith(item)) return true;
                }
                foreach (var item in InternationalPhoneNumbers.Split(","))
                {
                    if (phoneNumber.StartsWith(item)) return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidName(this string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return true;

                return Regex.Match(input, "^[a-zA-Z0-9_.-]*$").Success;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidEmail(this string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return true;

                return new EmailAddressAttribute().IsValid(input);
            }
            catch
            {
                return false;
            }
        }
    }
}