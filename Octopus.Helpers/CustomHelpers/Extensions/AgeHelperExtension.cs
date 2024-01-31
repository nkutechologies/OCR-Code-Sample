using System;

namespace Octopus.Helpers.CustomHelpers.Extensions
{
   public static class AgeHelperExtension
    {
        public static DateTime CalculateDOB(this string age)
        {
            // Trim any leading or trailing white spaces
            age = age.Trim();

            if (age.Contains("."))
            {
                age = age.Replace(".","");
            }

            string[] ageComponents = age.Split(' ');

            if (ageComponents.Length != 3)
            {
                throw new ArgumentException("Invalid age format. Please provide age in the format 'Y M D'.");
            }

            if (!int.TryParse(ageComponents[0].TrimEnd('Y'), out int years) ||
                !int.TryParse(ageComponents[1].TrimEnd('M'), out int months) ||
                !int.TryParse(ageComponents[2].TrimEnd('D'), out int days))
            {
                throw new ArgumentException("Invalid age format. Please provide valid numeric values for years, months, and days.");
            }

            DateTime currentDate = DateTime.Now;
            DateTime dob = currentDate
                .AddYears(-years)
                .AddMonths(-months)
                .AddDays(-days);

            return dob;
        }
    }
}
