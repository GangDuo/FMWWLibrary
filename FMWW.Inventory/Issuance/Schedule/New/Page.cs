using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Inventory.Issuance.Schedule.New
{
    // 在庫・棚卸[棚卸] -> 棚卸更新 -> 実行
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }
        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        private void Update2()
        {
            SignIn();

            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var _html = Encoding.UTF8.GetString(resData);

            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("F092_STOCKTAKING_UPDATE/F092_ENTRY.jsp");
            resData = _Client.UploadValues(address, PageContext.Translate());
            _html = Encoding.UTF8.GetString(resData);
            FMWW.Core.Helpers.Ajax.Run(this._Client, FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")));
        }
    }
}