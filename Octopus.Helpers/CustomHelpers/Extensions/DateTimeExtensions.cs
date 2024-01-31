using System;
using System.Globalization;

namespace Octopus.Helpers.CustomHelpers.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFormattedString(this DateTime date, string inputFormat, string outputFormat)
        {
            return DateTime.ParseExact(date.ToString(inputFormat), inputFormat, CultureInfo.InvariantCulture)
                            .ToString(outputFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime ParseExactInvariant(this string dateString, string format)
        {
            return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        }
    }

}
