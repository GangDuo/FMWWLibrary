using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ForShop.Customers.AdditionalPoint.New
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection()
            //    {
            //        {"form1:redirect",       "入力"},
            //        {"form1:redirectpage",   "S209_ENTRY.jsp"},
            //        {"form1:redirectfolder", "S209_POINT_MANUALLY_UPD"},
            //        {"form1:returnvalue",    "SUCCESS"},
            //        {"form1:functype",       "00"},
            //        {"form1:selectMenu",     "007"},
            //        {"form1:selectSubMenu",  "046"},
            //        {"form1:selectFunction", "S209"},
            //        {"form1:fop",            ""},
            //        {"form1",                "form1"}
            //    });
            return new MainMenu("S209_ENTRY.jsp",
                "S209_POINT_MANUALLY_UPD",
                "007",
                "046",
                "S209");
        }
    }
}
