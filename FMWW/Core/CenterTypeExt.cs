using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Core
{
    static public class CenterTypeExt
    {
        // Gender に対する拡張メソッドの定義
        public static string Code(this CenterType? s)
        {
            if (s == null)
            {
                return String.Empty;
            }
            string[] names = { "01", "02", "03", "99" };
            return names[(int)s];
        }
    }
}
