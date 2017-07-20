using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ScheduledArrival.DistributeExport.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "X024_SELECT.jsp"},
            //    {"form1:redirectfolder", "X024_160_DISTRIBUTE_EXPORT"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "10"},
            //    {"form1:selectMenu",     "004"},
            //    {"form1:selectSubMenu",  "007"},
            //    {"form1:selectFunction", "X024"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("X024_SELECT.jsp",
                        "X024_160_DISTRIBUTE_EXPORT",
                        "004",
                        "007",
                        "X024");
        }
    }
}
