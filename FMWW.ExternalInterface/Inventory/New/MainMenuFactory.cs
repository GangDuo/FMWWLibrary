using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ExternalInterface.Inventory.New
{
    internal class MainMenuFactory
    {
        public static MainMenu CreateInstance()
        {
            return new MainMenu(
                "E000_SELECT.jsp",
                "E000_IMPORT_GENERAL",
                "015",
                "041",
                "S311");
        }
    }
}
