using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Inventory.Issuance.Schedule.New
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "F092_ENTRY.jsp"},
            //    {"form1:redirectfolder", "F092_STOCKTAKING_UPDATE"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "80"},
            //    {"form1:selectMenu",     "010"},
            //    {"form1:selectSubMenu",  "031"},
            //    {"form1:selectFunction", "F092"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("F092_ENTRY.jsp",
                        "F092_STOCKTAKING_UPDATE",
                        "010",
                        "031",
                        "F092") { Functype = "80" };
        }
    }
}
