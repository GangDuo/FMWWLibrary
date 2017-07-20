using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Core.Script
{
    public class Parser
    {
        private static readonly string Pattern = @"message\[\d+\]\s*=\s*""(.+)""";

        public string Message { get; private set; }
        //public Dictionary<string, object> Variables { get; private set; }

        public void Parse(string javascript)
        {
            //Variables = new Dictionary<string, object>();
            var buf = new List<string>();
            MatchCollection m = Regex.Matches(javascript, Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            for (int i = 0; i < m.Count; i++)
            {
                buf.Add(m[i].Groups[1].Value);
            }
            Message = String.Join(Environment.NewLine, buf);
        }
    }
}
