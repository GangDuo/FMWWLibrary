using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ExternalInterface.Products.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection()
            //    {
            //        {"form1:redirect",       "入力"},
            //        {"form1:redirectpage",   "X021_SELECT.jsp"},
            //        {"form1:redirectfolder", "X021_080_PRODUCT_EXPORT"},
            //        {"form1:returnvalue",    "SUCCESS"},
            //        {"form1:functype",       "10"},
            //        {"form1:selectMenu",     "015"},
            //        {"form1:selectSubMenu",  "041"},
            //        {"form1:selectFunction", "X021"},
            //        {"form1:fop",            ""},
            //        {"form1",                "form1"}
            //    }
            return new MainMenu("X021_SELECT.jsp",
                            "X021_080_PRODUCT_EXPORT",
                            "015",
                            "041",
                            "X021");
        }
    }
}
