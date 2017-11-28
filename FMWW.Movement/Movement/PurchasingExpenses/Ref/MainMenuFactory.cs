using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.PurchasingExpenses.Ref
{
    class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            return new MainMenu("X038_SELECT.jsp",
                            "X038_160_MOVING_SUMMARY",
                            "011",
                            "032",
                            "X038");
        }
    }
}
