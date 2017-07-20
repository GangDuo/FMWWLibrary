using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Core.Mobile
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection()
            //{
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "X039_SELECT.jsp"},
            //    {"form1:redirectfolder", "X039_TABLET_ORDER"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "undefined"},
            //    {"form1:selectMenu",     ""},
            //    {"form1:selectSubMenu",  ""},
            //    {"form1:selectFunction", ""},
            //    {"form1:fop",            "X039_SELECT.jsp,X039_TABLET_ORDER,SUCCESS"},
            //    {"form1",                "form1"},
            //}
            return new MainMenu("X039_SELECT.jsp", "X039_TABLET_ORDER", "", "", "")
            {
                Functype = "undefined",
                Fop = "X039_SELECT.jsp,X039_TABLET_ORDER,SUCCESS"
            };
        }
    }
}
