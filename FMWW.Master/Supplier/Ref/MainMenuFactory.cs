using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Master.Supplier.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "M040_SELECT.jsp"},
            //    {"form1:redirectfolder", "M040_SUPPLIER"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "10"},
            //    {"form1:selectMenu",     "013"},
            //    {"form1:selectSubMenu",  "036"},
            //    {"form1:selectFunction", "M040"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("M040_SELECT.jsp",
                "M040_SUPPLIER",
                "013",
                "036",
                "M040");
        }
    }
}
