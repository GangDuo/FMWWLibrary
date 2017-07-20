using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ForShop.Customers.AdditionalPoint.New
{
    // 店舗管理 [店舗顧客] -> ポイント入力 -> 入力
    class Page : FMWW.Http.Page
    {
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            byte[] resData = this._Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(resData);
        }

        public override void Register()
        {
            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X086_160_PROD_SHELF/X086_SELECT.jsp");
            //resData = this.UploadValues(address,
            //    new System.Collections.Specialized.NameValueCollection() {
            //        {"form1:execute",    "execute"},
            //        {"form1:action",     "export"},
            //        {"form1:isAjaxMode", "1"},
            //        {"form1",            "form1"},
            //        {"cache",            Core.UnixEpochTime.now().ToString()},
            //    });
            //_html = System.Text.Encoding.UTF8.GetString(resData);
            base.Register();
        }
    }
}
