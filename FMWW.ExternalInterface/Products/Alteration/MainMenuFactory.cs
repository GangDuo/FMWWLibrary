using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ExternalInterface.Products.Alteration
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            // form1:redirect	入力
            // form1:redirectpage	E000_SELECT.jsp
            // form1:redirectfolder	E000_IMPORT_GENERAL
            // form1:returnvalue	SUCCESS
            // form1:functype	00
            // form1:selectMenu	015
            // form1:selectSubMenu	041
            // form1:selectFunction	X041
            // form1:fop	
            // form1	form1
            return new MainMenu(
                "E000_SELECT.jsp",
                "E000_IMPORT_GENERAL",
                "015",
                "041",
                "X041")
            {
                Functype = "00"
            };
        }
    }
}
