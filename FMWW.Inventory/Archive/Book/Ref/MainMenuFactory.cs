using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Inventory.Archive.Book.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "S300_SELECT.jsp"},
            //    {"form1:redirectfolder", "S300_STOCK"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "10"},
            //    {"form1:selectMenu",     "010"},
            //    {"form1:selectSubMenu",  "030"},
            //    {"form1:selectFunction", "S300"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("S300_SELECT.jsp",
                        "S300_STOCK",
                        "010",
                        "030",
                        "S300");
        }
    }
}
