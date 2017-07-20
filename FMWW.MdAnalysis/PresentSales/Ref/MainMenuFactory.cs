using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.MdAnalysis.PresentSales.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            // form1:redirect	    入力
            // form1:redirectpage	NA250_SELECT.jsp
            // form1:redirectfolder	NA250_SALES_PRESENT_TTL
            // form1:returnvalue	SUCCESS
            // form1:functype	    10
            // form1:selectMenu	    008
            // form1:selectSubMenu	021
            // form1:selectFunction	NA25
            // form1:fop	
            // form1	            form1
            return new MainMenu("NA250_SELECT.jsp",
                            "NA250_SALES_PRESENT_TTL",
                            "008",
                            "021",
                            "NA25");
        }
    }
}
