using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Core
{
    public class MainMenu
    {
        public static readonly Uri Url = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("index.jsp");

        public string Redirect { get; set; }
        public string RedirectPage { get; set; }
        public string RedirectFolder { get; set; }
        public string Returnvalue { get; set; }
        public string Functype { get; set; }
        public string SelectMenu { get; set; }
        public string SelectSubMenu { get; set; }
        public string SelectFunction { get; set; }
        public string Fop { get; set; }
        public string Form1 { get; set; }

        public NameValueCollection Translate()
        {
            return new NameValueCollection()
                {
		            {"form1:redirect",       this.Redirect},
		            {"form1:redirectpage",   this.RedirectPage},
		            {"form1:redirectfolder", this.RedirectFolder},
		            {"form1:returnvalue",    this.Returnvalue},
		            {"form1:functype",       this.Functype},
		            {"form1:selectMenu",     this.SelectMenu},
		            {"form1:selectSubMenu",  this.SelectSubMenu},
		            {"form1:selectFunction", this.SelectFunction},
		            {"form1:fop",            this.Fop},
		            {"form1",                this.Form1}
                };
        }

        public MainMenu(string redirectpage, string redirectfolder, string selectMenu, string selectSubMenu, string selectFunction)
        {
            this.Redirect = "入力";
            this.Returnvalue = "SUCCESS";
            this.Functype = "10";
            this.Fop = "";
            this.Form1 = "form1";

            this.RedirectPage = redirectpage;
            this.RedirectFolder = redirectfolder;
            this.SelectMenu = selectMenu;
            this.SelectSubMenu = selectSubMenu;
            this.SelectFunction = selectFunction;
        }
    }
}
