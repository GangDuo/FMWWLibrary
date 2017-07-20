using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Master.Shelf.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //new NameValueCollection() {
            //    {"form1:redirect",       "入力"},
            //    {"form1:redirectpage",   "X086_SELECT.jsp"},
            //    {"form1:redirectfolder", "X086_160_PROD_SHELF"},
            //    {"form1:returnvalue",    "SUCCESS"},
            //    {"form1:functype",       "10"},
            //    {"form1:selectMenu",     "013"},
            //    {"form1:selectSubMenu",  "035"},
            //    {"form1:selectFunction", "X086"},
            //    {"form1:fop",            ""},
            //    {"form1",                "form1"}
            //}
            return new MainMenu("X086_SELECT.jsp",
                        "X086_160_PROD_SHELF",
                        "013",
                        "035",
                        "X086");
        }
    }
}
