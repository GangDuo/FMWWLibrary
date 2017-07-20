using FMWW.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Core
{
    abstract public class AbstractAuthentication
    {
        public static readonly string HostName = LoadHostName();
        protected static readonly Encoding ShiftJIS = Encoding.GetEncoding("Shift_JIS");

        public event SignedInEventHandler SignedIn;
        protected virtual void OnSignedIn(SignedInEventArgs e)
        {
            if (SignedIn != null)
                SignedIn(this, e);
        }

        private static string LoadHostName()
        {
            using (var sr = new StreamReader(".hostname.json"))
            {
                var text = sr.ReadToEnd();
                if (text.Length == 0)
                {
                    throw new Exception("Host未設定");
                }
                else
                {
                    return Text.Json.Parse<string>(text);
                }
            }
        }
    }
}
