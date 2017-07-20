using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Ordering.Shop.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            //{"form1:redirect",	    "入力"},
            //{"form1:redirectpage",	"X023_SELECT.jsp"},
            //{"form1:redirectfolder",	"X023_160_ORDER_EXPORT"},
            //{"form1:returnvalue",	    "SUCCESS"},
            //{"form1:functype",	    "00"},
            //{"form1:selectMenu",	    "003"},
            //{"form1:selectSubMenu",	"006"},
            //{"form1:selectFunction",	"X023"},
            //{"form1:fop",	            ""},
            //{"form1",	                "form1"},
            return new MainMenu("X023_SELECT.jsp",
                            "X023_160_ORDER_EXPORT",
                            "003",
                            "006",
                            "X023") { Functype = "00" };
        }
    }
}
