using FMWW.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Master.PriceTag.New
{
    public class Page : FMWW.Http.AbstractUploader
    {
       // public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override void Register()
        {
            Execute(null);
        }

        protected override MainMenu SelectMainMenu()
        {
            return MainMenuFactory.CreateInstance();
        }
    }
}
