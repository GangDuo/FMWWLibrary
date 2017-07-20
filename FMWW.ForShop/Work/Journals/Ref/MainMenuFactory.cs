using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ForShop.Work.Journals.Ref
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            return new MainMenu("S086_SELECT.jsp",
                            "S086_SHOP_SALES_REPORT",
                            "007",
                            "017",
                            "S086");
        }
    }
}
