using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.PurchasingExpenses.Ref
{
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override byte[] Excel()
        {
            SignIn();

            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
#if DEBUG
            var html = Encoding.UTF8.GetString(resData);
            Debug.WriteLine(html);
#endif

            var context = PageContext ?? new Context();
            byte[] bin = this._Client.UploadValues(
                FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X038_160_MOVING_SUMMARY/X038_SELECT.jsp"),
                context.Translate());


            return bin;
        }
    }
}
