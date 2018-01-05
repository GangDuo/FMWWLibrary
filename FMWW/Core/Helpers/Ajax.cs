using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Core.Helpers
{
    public class Ajax
    {
        public static string TimeStamp()
        {
            return DateTime.Now.ToString("ddd MMM dd HH:mm:ss UTCzz00 yyyy", new CultureInfo("en-US"));
        }

        public static bool IsFin(string text)
        {
            return Regex.IsMatch(text, "isFin[^=]*=[^;]*true", RegexOptions.IgnoreCase);
        }

        public static bool HasError(string text)
        {
            return Regex.IsMatch(text, @"setError\([^\)]*\);", RegexOptions.IgnoreCase);
        }

        public static string SnipError(string text)
        {
            var m = Regex.Match(text, @"setError\(([^\)]*)\);");
            if (m.Groups.Count < 2)
            {
                return String.Empty;
            }
            return m.Groups[1].Value.Trim(new char[] { '"', '\'' });
        }

        public static NameValueCollection CreateAjaxQuery()
        {
            return new NameValueCollection()
            {
                {"form1:isAjaxMode", "1"},
                {"form1",            "form1"},
                {"form1:execute",    "execute"},
                {"cache",            FMWW.Utility.UnixEpochTime.now().ToString()},
            };
        }

        public static void Run(FMWW.Http.Client client, Uri address)
        {
            var text = "";
            while (!IsFin(text))
            {
                var resData = client.UploadValues(address, CreateAjaxQuery());
                text = Encoding.UTF8.GetString(resData);
                if (HasError(text))
                {
                    throw new Exception(SnipError(text));
                }
            }
        }
    }
}
