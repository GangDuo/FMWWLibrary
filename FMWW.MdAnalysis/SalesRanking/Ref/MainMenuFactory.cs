using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.MdAnalysis.SalesRanking.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            // {"form1:redirect",       "入力"},
            // {"form1:redirectpage",   "A210_SELECT.jsp"},
            // {"form1:redirectfolder", "A210_SALES_RANKING"},
            // {"form1:returnvalue",    "SUCCESS"},
            // {"form1:functype",       "10"},
            // {"form1:selectMenu",     "008"},
            // {"form1:selectSubMenu",  "021"},
            // {"form1:selectFunction", "A210"},
            // {"form1:fop",            ""},
            // {"form1",                "form1"},
            return new MainMenu("A210_SELECT.jsp",
                            "A210_SALES_RANKING",
                            "008",
                            "021",
                            "A210");
        }
    }
}
