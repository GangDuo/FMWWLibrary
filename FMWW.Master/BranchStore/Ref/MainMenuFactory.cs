using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Master.BranchStore.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "M052_SELECT.jsp"},
            //    {"form1:redirectfolder", "M052_DESTINATION"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "10"},
            //    {"form1:selectMenu",     "013"},
            //    {"form1:selectSubMenu",  "036"},
            //    {"form1:selectFunction", "M052"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("M052_SELECT.jsp",
                        "M052_DESTINATION",
                        "013",
                        "036",
                        "M052");
        }
    }
}
