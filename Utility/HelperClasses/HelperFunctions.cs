using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Utility.ProjeGlobal;

namespace Utility.HelperClasses
{
    public static class HelperFunctions
    {
        public static bool GetResolvedConnecionIpAddress(string serverNameOrUrl, out string resolvedIpAddress)
        {
            var isResolved = false;
            IPAddress resolvIp = null;
            try
            {
                if (!IPAddress.TryParse(serverNameOrUrl, out resolvIp))
                {
                    var hostEntry = Dns.GetHostEntry(serverNameOrUrl);

                    if (hostEntry != null && hostEntry.AddressList != null
                        && hostEntry.AddressList.Length > 0)
                    {
                        if (hostEntry.AddressList.Length == 1)
                        {
                            resolvIp = hostEntry.AddressList[0];
                            isResolved = true;
                        }
                        else
                        {
                            foreach (var var in hostEntry.AddressList.Where(var => var.AddressFamily == AddressFamily.InterNetwork))
                            {
                                resolvIp = var;
                                isResolved = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    isResolved = true;
                }
            }
            catch (Exception)
            {
                isResolved = false;
                resolvIp = null;
            }
            finally
            {
                if (resolvIp != null) resolvedIpAddress = resolvIp.ToString();
            }

            resolvedIpAddress = null;
            return isResolved;
        }

        public static string SerializeObject<T>(T source)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, source);
                return sw.ToString();
            }
        }

        public static T DeSerializeObject<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }

        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }

        public static object ReturnFalseIfNull(this object value)
        {
            if (value == DBNull.Value)
                return false;
            if (value == null)
                return false;
            return value;
        }

        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }

        public static object ReturnNullIfDbNull(this object value)
        {
            if (value == DBNull.Value)
                return '\0';
            if (value == null)
                return '\0';
            return value;
        }
        public static object ReturnEmptyIfZeroOrNull(this object value)
        {
            if (value.ReturnEmptyIfNull().ToString().Equals("0"))
                return string.Empty;
            return value;
        }

        //This function formats the display-name of a user,
        //and removes unnecessary extra information.
        public static string FormatUserDisplayName(string displayName = null, string defaultValue = "tBill Users",
            bool returnNameIfExists = false, bool returnAddressPartIfExists = false)
        {
            //Get the first part of the Users's Display Name if s/he has a name like this: "firstname lastname (extra text)"
            //removes the "(extra text)" part
            if (!string.IsNullOrEmpty(displayName))
            {
                if (returnNameIfExists)
                    return Regex.Replace(displayName, @"\ \(\w{1,}\)", "");
                return (displayName.Split(' '))[0];
            }
            if (returnAddressPartIfExists)
            {
                var emailParts = defaultValue.Split('@');
                return emailParts[0];
            }
            return defaultValue;
        }

        public static string FormatUserTelephoneNumber(this string telephoneNumber)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(telephoneNumber))
            {
                //result = telephoneNumber.ToLower().Trim().Trim('+').Replace("tel:", "");
                result = telephoneNumber.ToLower().Trim().Replace("tel:", "");

                if (result.Contains(";"))
                {
                    if (!result.ToLower().Contains(";ext="))
                        result = result.Split(';')[0];
                }
            }

            return result;
        }

        public static bool IsValidEmail(this string emailAddress)
        {
            const string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            return Regex.IsMatch(emailAddress, pattern);
        }

        /// <summary>
        /// Convert DateTime to string
        /// </summary>
        /// <param name="datetTime"></param>
        /// <param name="excludeHoursAndMinutes">if true it will execlude time from datetime string. Default is false</param>
        /// <returns></returns>
        public static string ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                    return datetTime.ToString("yyyy-MM-dd");
                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }


        public static string ConvertSecondsToReadable(this int secondsParam)
        {
            var hours = Convert.ToInt32(Math.Floor((double)(secondsParam / 3600)));
            var minutes = Convert.ToInt32(Math.Floor((double)(secondsParam - (hours * 3600)) / 60));
            var seconds = secondsParam - (hours * 3600) - (minutes * 60);

            var hoursStr = hours.ToString();
            var minsStr = minutes.ToString();
            var secsStr = seconds.ToString();

            if (hours < 10)
            {
                hoursStr = "0" + hoursStr;
            }

            if (minutes < 10)
            {
                minsStr = "0" + minsStr;
            }
            if (seconds < 10)
            {
                secsStr = "0" + secsStr;
            }

            return hoursStr + ':' + minsStr + ':' + secsStr;
        }


        public static string ConvertSecondsToReadable(this long secondsParam)
        {
            var hours = Convert.ToInt32(Math.Floor((double)(secondsParam / 3600)));
            var minutes = Convert.ToInt32(Math.Floor((double)(secondsParam - (hours * 3600)) / 60));
            var seconds = Convert.ToInt32(secondsParam - (hours * 3600) - (minutes * 60));

            var hoursStr = hours.ToString();
            var minsStr = minutes.ToString();
            var secsStr = seconds.ToString();

            if (hours < 10)
            {
                hoursStr = "0" + hoursStr;
            }

            if (minutes < 10)
            {
                minsStr = "0" + minsStr;
            }
            if (seconds < 10)
            {
                secsStr = "0" + secsStr;
            }

            return hoursStr + ':' + minsStr + ':' + secsStr;
        }

        public static object ReturnQuotedValue(this object value)
        {

            if (value == DBNull.Value || value == null)
            {
                value = "'" + string.Empty + "'";
            }
            else
            {
                value = "'" + value.ToString().Replace('\'', ' ') + "'"; //string içinde ' karakteri geçiyorsa hata aliniyor. O nedenle ' karakteri bosluk ile degistiriliyor.
            }
            return value;
        }
        public static object ReturnDoubleQuotedValue(this object value)
        {

            if (value == DBNull.Value || value == null)
            {
                value = "\"" + string.Empty + "\"";
            }
            else
            {
                value = "\"" + value.ToString().Replace('\'', ' ') + "\""; //string içinde ' karakteri geçiyorsa hata aliniyor. O nedenle ' karakteri bosluk ile degistiriliyor.
            }
            return value;
        }
        public static object ReturnNullStringValue(this object value)
        {

            if (value == DBNull.Value || value == null)
            {
                value = "null";
            }
            else
            {
                value = value.ReturnQuotedValue();
            }
            return value;
        }

        public static string ReturnTRDateFormat(this DateTime value)
        {
            string stringValue = string.Empty;
            if (value == null || value == DateTime.MinValue)
            {
                stringValue = "'" + stringValue + "'";
            }
            else
            {
                //stringValue = "'" + value.ToString("yyyy-MM-dd H:mm:ss") + "'";
                //CultureInfo cif = new CultureInfo(ProjeConstants.CULTUREINFO);

                stringValue = "'" + value.ToString() + "'";
                //stringValue = "'2018-01-02'";
            }
            return stringValue;
        }
        public static string ReturnDDMMYYYFormat(this DateTime value)
        {
            value = value.AddDays(1).AddSeconds(-1);
            string stringValue = string.Empty;
            if (value == null || value == DateTime.MinValue)
            {
                stringValue = "'" + stringValue + "'";
            }
            else
            {
                //stringValue = "'" + value.ToString("yyyy-MM-dd H:mm:ss") + "'";
                //CultureInfo cif = new CultureInfo(ProjeConstants.CULTUREINFO);

                stringValue = "'" + value.ToString("dd.MM.yyyy") + "'";
                //stringValue = "'2018-01-02'";
            }
            return stringValue;
        }
        public static string ConvertToDDMMYYYHHmmFormat(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;

            DateTime result = new DateTime();

            bool isConverted = DateTime.TryParse(value.ReturnDateTimeMinIfNull().ToString(), out result);

            if (!isConverted)
            {
                DateTime.TryParseExact(value.ReturnDateTimeMinIfNull().ToString(), @"dd.MM.yyyy HH:mm", new CultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out result);
            }
            DateTime minValue = new DateTime(1900, 01, 01);

            string retval = result == minValue ? string.Empty : result.Equals(DateTime.MinValue) ? string.Empty : result.ToString("dd.MM.yyyy HH:mm");

            return retval;
        }
        public static int ConvertToInt(this object value)
        {
            int result = 0;

            bool isConverted = int.TryParse(value.ReturnZeroIfNull().ToString(), out result);

            return result;
        }

        public static long ConvertToLong(this object value)
        {
            long result = 0;
            value = value.ToString().Replace(" ", "");
            bool isConverted = long.TryParse(value.ReturnZeroIfNull().ToString(), out result);

            return result;
        }
        public static string ConvertDecimalToString(this object value)
        {

            string input = value.ReturnZeroIfNull().ToString();

            // remove empty spaces
            input = input.Replace(" ", "");
            // checks if the string is empty
            if (string.IsNullOrEmpty(input) == false)
            {
                // check if input has , and . for thousands separator and decimal place
                if (input.Contains(",") && input.Contains("."))
                {
                    // find the decimal separator, might be , or .
                    int decimalpos = input.LastIndexOf(',') > input.LastIndexOf('.') ? input.LastIndexOf(',') : input.LastIndexOf('.');
                    // uses | as a temporary decimal separator
                    input = input.Substring(0, decimalpos) + "|" + input.Substring(decimalpos + 1);
                    // formats the output removing the , and . and replacing the temporary | with .
                    input = input.Replace(".", "").Replace(",", "").Replace("|", ".");
                }
                // replaces , with .
                if (input.Contains(","))
                {
                    input = input.Replace(',', '.');
                }
                // checks if the input number has thousands separator and no decimal places
                if (input.Count(item => item == '.') > 1)
                {
                    input = input.Replace(".", "");
                }

            }
            return input;
        }
        public static decimal ConvertToDecimal(this object value)
        {
            decimal result = 0;
            try
            {
                string input = value.ReturnZeroIfNull().ToString();

                // remove empty spaces
                input = input.Replace(" ", "");

                // checks if the string is empty
                if (string.IsNullOrEmpty(input) == false)
                {
                    if (input.Contains(".") && !input.Contains(","))
                    {
                        input = input.Replace(".", "");
                    }
                    // check if input has , and . for thousands separator and decimal place
                    if (input.Contains(",") && input.Contains("."))
                    {
                        // find the decimal separator, might be , or .
                        int decimalpos = input.LastIndexOf(',') > input.LastIndexOf('.') ? input.LastIndexOf(',') : input.LastIndexOf('.');
                        // uses | as a temporary decimal separator
                        input = input.Substring(0, decimalpos) + "|" + input.Substring(decimalpos + 1);
                        // formats the output removing the , and . and replacing the temporary | with .
                        input = input.Replace(".", "").Replace(",", "").Replace("|", ".");
                    }
                    // replaces , with .
                    if (input.Contains(","))
                    {
                        input = input.Replace(',', '.');
                    }
                    // checks if the input number has thousands separator and no decimal places
                    if (input.Count(item => item == '.') > 1)
                    {
                        input = input.Replace(".", "");
                    }

                    // tries to convert input to double
                    if (decimal.TryParse(input, out result) == true)
                    {
                        result = decimal.Parse(input, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("Decimal hatasi alindi.ConvertToDecimal methodu ", ex);
                throw exception;
            }
            return result;
        }

        public static DateTime ConvertToDatetime(this object value)
        {
            DateTime result = new DateTime();

            bool isConverted = DateTime.TryParse(value.ReturnDateTimeMinIfNull().ToString(), out result);

            if (!isConverted)
            {
                DateTime.TryParseExact(value.ReturnDateTimeMinIfNull().ToString(), @"dd.MM.yyyy HH:mm", new CultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out result);
            }

            return result;
        }
        public static TimeSpan ConvertToTimeSpan(this object value)
        {
            TimeSpan result = new TimeSpan();

            bool isConverted = TimeSpan.TryParse(value.ReturnZeroIfNull().ToString(), out result);

            if (!isConverted)
            {
                TimeSpan.TryParseExact(value.ReturnZeroIfNull().ToString(), @"HH:mm", new CultureInfo(ProjeConstants.CULTUREINFO), TimeSpanStyles.None, out result);
            }

            return result;
        }
        public static string ConvertToTimeSpanReturnInHHmm(this object value)
        {
            TimeSpan result = new TimeSpan();

            bool isConverted = TimeSpan.TryParse(value.ReturnZeroIfNull().ToString(), out result);

            if (!isConverted)
            {
                TimeSpan.TryParseExact(value.ReturnZeroIfNull().ToString(), @"HH:mm", new CultureInfo(ProjeConstants.CULTUREINFO), TimeSpanStyles.None, out result);
            }
            string retvalHHmm = string.Empty;
            string minus = string.Empty;
            if (result.Ticks < 0)
            {
                result = result.Duration();
                minus = "-";
            }
            TimeSpan minTs = new TimeSpan(0, 1, 0);
            string dakika = result.Minutes == 0 ? "" : result.Minutes.ToString() + " Dk.";
            string saat = result.Hours == 0 ? "" : result.Hours.ToString() + " Saat.";
            int totalHours = (int)result.TotalHours;
            string totalHoursStr = totalHours > 0 ? totalHours.ToString() + " Saat " : "";
            retvalHHmm = minus + totalHoursStr + dakika;
            return retvalHHmm;
        }
        public static string ConvertToDatetimeEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;

            DateTime result = new DateTime();

            bool isConverted = DateTime.TryParse(value.ReturnDateTimeMinIfNull().ToString(), out result);

            if (!isConverted)
            {
                DateTime.TryParseExact(value.ReturnDateTimeMinIfNull().ToString(), @"dd.MM.yyyy HH:mm", new CultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out result);
            }
            DateTime minValue = new DateTime(1900, 01, 01);

            string retval = result == minValue ? string.Empty : result.Equals(DateTime.MinValue) ? string.Empty : result.ToString("dd.MM.yyyy");

            return retval;
        }
        public static DateTime ConvertToDatetime(this object value, CultureInfo culture)
        {
            DateTime result = new DateTime();

            bool isConverted = DateTime.TryParse(value.ReturnDateTimeMinIfNull().ToString(), culture, DateTimeStyles.None, out result);

            return result;
        }

        public static List<string> ConvertTextFileToList(Stream fileStream)
        {
            // Dictionary<string, List<string>> linesInfo = new Dictionary<string, List<string>>();

            List<string> lines = new List<string>();
            string line = string.Empty;
            using (StreamReader file = new StreamReader(fileStream, Encoding.GetEncoding("windows-1254")))
            {
                while ((line = file.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }
        public static List<string> ConvertTextFileToList(Stream fileStream, Encoding encoding)
        {
            List<string> lines = new List<string>();
            string line = string.Empty;

            try
            {
                using (var streamReader = new StreamReader(fileStream, encoding))
                {
                    line = String.Empty;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
            return lines;
        }
        public static bool ConvertToBool(this object value)
        {
            bool result = false;

            bool isConverted = bool.TryParse(value.ReturnFalseIfNull().ToString(), out result);

            return result;
        }
        public static string ReplaceTrChars(this object value)
        {
            string newValue = value.ReturnZeroIfNull().ToString();
            //DateTime dt = DateTime.Today;
            newValue = newValue.Replace(" ", "");
            newValue = newValue.Replace("Ç", "C");
            newValue = newValue.Replace("ç", "c");
            newValue = newValue.Replace("G", "G");
            newValue = newValue.Replace("g", "g");
            newValue = newValue.Replace("I", "I");
            newValue = newValue.Replace("i", "i");
            newValue = newValue.Replace("Ö", "O");
            newValue = newValue.Replace("ö", "o");
            newValue = newValue.Replace("S", "S");
            newValue = newValue.Replace("s", "s");
            newValue = newValue.Replace("Ü", "U");
            newValue = newValue.Replace("ü", "u");
            return newValue;
        }
    }
}