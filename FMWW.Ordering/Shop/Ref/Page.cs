using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Ordering.Shop.Ref
{
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override string Csv()
        {
            var client = this._Client;
            SignIn();

            client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());

            // 発注検索
            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X023_160_ORDER_EXPORT/X023_SELECT.jsp");
            var result = client.UploadValues(address, this.PageContext.Translate(true));
            var html = Encoding.UTF8.GetString(result);

            // ajax
            FMWW.Core.Helpers.Ajax.Run(this._Client, FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")));

            // ファイルダウンロード
            result = client.UploadValues(address, this.PageContext.Translate());
            var raw = Encoding.GetEncoding("Shift_JIS").GetString(result);
            return raw;
        }
    }
}
