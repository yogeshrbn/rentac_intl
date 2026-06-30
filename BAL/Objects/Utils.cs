using BAL.DAL;
using Microsoft.Owin;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
namespace BAL.Objects
{
    public class Utils
    {
        static CultureInfo culture = new CultureInfo("en-GB");
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string _containerBaseUrl = ConfigurationManager.AppSettings["ContainerBaseUrl"];
        private static string storage = ConfigurationManager.AppSettings["storage"];

        public static DateTime CurrentDate()
        {
            return DateTime.Today;
        }
        public static DateTime FormatDate(string date)
        {
            //date = date.Remove(10).Trim();
            if (date.Length > 10)
            {
                date = date.Remove(10);
            }
            date = date.Replace("-", "/");
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", culture);
            string s = dt.ToString("dd/MM/yyyy", culture);
            return dt;
        }
        public static String CheckDateAndReturnDefault(string date, string defaultValue = "NA")
        {
            //date = date.Remove(10).Trim();

            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                // Optional: Additional validation for specific range
                if (parsedDate.Year >= 1900 && parsedDate.Year <= 9999)
                {
                    return parsedDate.ToString("dd/MM/yyyy");
                }
            }
            return defaultValue;

        }

        public static string SanitizeFileName(string fileName, string replaceWith = "_")
        {
            //   using System.Text.RegularExpressions;


            // Keep only alphanumeric characters and hyphens
            var sanitizedfileName = Regex.Replace(fileName, @"[^a-zA-Z0-9\-]", replaceWith);
            return sanitizedfileName;
        }

        public static string NumberValueFromString(string numberString)
        {

            return new string(numberString.Where(char.IsDigit).ToArray());
        }
        public static String FormatDate(DateTime date)
        {

            return date.ToString("dd/MM/yyyy");


        }
        public String DateToDDMMYYYY(DateTime date)
        {
            if (date.Year <= 1900)
            {
                return "";
            }
            return date.ToString("dd/MM/yyyy");
        }

        public String DateToDDMMYYYY(DateTime date, string defaultIfEmpty = "")
        {
            if (date.Year <= 1900)
            {
                return defaultIfEmpty;
            }
            return date.ToString("dd/MM/yyyy");
        }

        //public String DateToDDMMYYYY(string dateStr)
        //{
        //    if (string.IsNullOrWhiteSpace(dateStr)) return "";
        //    if (!DateTime.TryParse(dateStr, culture, System.Globalization.DateTimeStyles.None, out DateTime date))
        //        return dateStr;
        //    if (date.Year <= 1900) return "";
        //    return date.ToString("dd/MM/yyyy");
        //}

        //public String DateToDDMMYYYY(object value)
        //{
        //    if (value == null) return "";
        //    return DateToDDMMYYYY(value.ToString());
        //}

        public static string RemoveTime(string date)
        {
            return Utils.FormatDate(Convert.ToDateTime(date, culture));

        }

        public static string ConvertNumbertoWords(String number)
        {
            return AmountToWords(Convert.ToDecimal(number));
        }
        public static string QtyToWords(decimal amount)
        {
            int rupees = (int)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);

            string words = NumberToWords(rupees);

            if (paise > 0)
            {
                words += " and " + NumberToWords(paise) + " Paise";
            }

            return words;
        }
        public static string AmountToWords(decimal amount)
        {
            int rupees = (int)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);

            string words = "Rupees " + NumberToWords(rupees);

            if (paise > 0)
            {
                words += " and " + NumberToWords(paise) + " Paise";
            }

            return words + " Only";
        }
        public static string NumberToWords(int number)
        {
            if (number == 0) return "Zero";

            if (number < 0) return "Minus " + NumberToWords(Math.Abs(number));

            string[] unitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                          "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }
            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "") words += "and ";
                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        public static string FormatNumber(double number)
        {
            return number.ToString("#,##0.00");
        }

        public static int ToInt(decimal number)
        {
            int size;

            return Convert.ToInt32(number) ;
        }



        static readonly string uniqIdLock;
        public static string GetUniqueId()
        {
            string uniqueId = "";

            uniqueId = Convert.ToString(DateTime.Now.Ticks - Convert.ToDateTime("01/01/2023").Ticks);

            return uniqueId;
        }

        public static decimal CalculateReverseGST(decimal amount, decimal rate)
        {


            return amount / (1 + rate / 100);


        }
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            // Ensure startDate is earlier than endDate for consistent calculation
            if (startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            int months = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);

            // Adjust if the end date's day is earlier in the month than the start date's day
            if (endDate.Day < startDate.Day)
            {
                months--;
            }

            return months;
        }

        public static decimal RoundOff(decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

        public static double GetDaysDifference(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;

            // Get the total number of days from the TimeSpan
            return difference.TotalDays;
        }
        public static string GetEnumValue<T>(T enumValue) where T : Enum
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
            return ((EnumMemberAttribute)attributes[0]).Value;
        }

        public static int DaysInMonth(DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month); ;
        }
        public static DateTime LastDaysOfMonth(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;

            // Create a DateTime object for the first day of the specified month
            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            // Add one month to get the first day of the next month
            DateTime firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            // Subtract one day to get the last day of the current month
            return firstDayOfNextMonth.AddDays(-1);
        }
        public static string GetExtension(string mimeType)
        {
            char delimiter = '/';
            var segments = mimeType.Split(delimiter);
            return segments[segments.Length - 1];
        }

        public static bool IsValidJson(string jsonString)
        {
            try
            {
                using (JsonDocument.Parse(jsonString))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
            catch (Exception) // Catch other potential exceptions if needed
            {
                return false;
            }
        }

        public static string getDocs(int compId, string fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                return _containerBaseUrl + "0/" + compId + "/docs/" + fileName;
            }
            return "";
        }
    }


    public class Utils<T>
    {
        public List<T> ConstructList(System.Data.DataSet ds)
        {
            SQL objsql = new SQL();
            return objsql.ContructList<T>(ds);
        }
    }

    public class DataUrlHelper
    {
        public string ContentType { get; set; }
        public string Extension { get; set; }

        public Stream FileStream { get; set; }

        public bool IsDataUrl(string dataUrl)
        {
            try
            {
                var matches = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)");

                if (matches.Groups.Count < 3)
                {
                    throw new Exception("Invalid DataUrl format");
                }

                ContentType = matches.Groups["type"].Value;
                Extension = GetExtension(ContentType);
                FileStream = new MemoryStream(Convert.FromBase64String(matches.Groups["data"].Value));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void Parse(string dataUrl)
        {
            var matches = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)");

            if (matches.Groups.Count < 3)
            {
                throw new Exception("Invalid DataUrl format");
            }

            ContentType = matches.Groups["type"].Value;
            Extension = GetExtension(ContentType);
            FileStream = new MemoryStream(Convert.FromBase64String(matches.Groups["data"].Value));
        }

        public string GetExtension(string mimeType)
        {
            char delimiter = '/';
            var segments = mimeType.Split(delimiter);
            return segments[segments.Length - 1];
        }
    }
}
