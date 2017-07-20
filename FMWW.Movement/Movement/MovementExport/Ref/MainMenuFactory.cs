using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.MovementExport.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            // {"form1:redirect",       "入力"},
            // {"form1:redirectpage",   "F065_SELECT.jsp"},
            // {"form1:redirectfolder", "F065_MOVE_EXPORT"},
            // {"form1:returnvalue",    "SUCCESS"},
            // {"form1:functype",       "10"},
            // {"form1:selectMenu",     "011"},
            // {"form1:selectSubMenu",  "032"},
            // {"form1:selectFunction", "F065"},
            // {"form1:fop",            ""},
            // {"form1",                "form1"}
            return new MainMenu("F065_SELECT.jsp",
                            "F065_MOVE_EXPORT",
                            "011",
                            "032",
                            "F065");
        }
    }
}
