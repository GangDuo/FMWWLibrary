using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.MovementExport.Ref
{
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
#if DEBUG
            var html = Encoding.UTF8.GetString(resData);
            Debug.WriteLine(html);
#endif
        }

        public override void Quit()
        {
            throw new NotImplementedException();
        }

        public override string Csv()
        {
            SignIn();
            Reach();
            var context = PageContext ?? new Context();
            byte[] resData = this._Client.UploadValues(
                FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("F065_MOVE_EXPORT/F065_SELECT.jsp"),
                context.Translate());
            return Encoding.GetEncoding("shift_jis").GetString(resData);
        }
    }
}
