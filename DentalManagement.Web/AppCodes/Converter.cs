
using System.Globalization;

namespace DentalManagement.Web.AppCodes
{
    public static class Converter
    {
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);

            }
            catch
            {
                return null;
            }
        }

    }
}
