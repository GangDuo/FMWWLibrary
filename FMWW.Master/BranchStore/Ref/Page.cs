using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Master.BranchStore.Ref
{
    // ﾏｽﾀ -> 納入先ﾏｽﾀｰ -> 照会
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override byte[] Excel()
        {
            SignIn();

            byte[] resData = this._Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(resData);

            resData = this._Client.UploadValues(
                FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("M052_DESTINATION/M052_SELECT.jsp"),
                (new Context()).Translate());
            html = Encoding.UTF8.GetString(resData);

            do
            {
                resData = this._Client.UploadValues(UrlSlipList,
                    new NameValueCollection() {
                    {"row",      "40"},
                    {"dispMode", "null"},
                    {"index",    "1"},
                    {"cache",    FMWW.Utility.UnixEpochTime.now().ToString()},
                });
                html = Encoding.UTF8.GetString(resData);
            } while (!FMWW.Core.Helpers.Ajax.IsFin(html));

            resData = this._Client.UploadValues(UrlSlipList,
                new NameValueCollection() {
                    {"action", "excel"},
                    {"row",    "0"},
                    {"index",  "1"},
                    {"cache",  FMWW.Utility.UnixEpochTime.now().ToString()},
                });
            html = Encoding.UTF8.GetString(resData);
            if (!FMWW.Core.Helpers.Ajax.IsFin(html))
            {
                throw new Exception();
            }

            resData = this._Client.UploadValues(UrlSlipList,
                new NameValueCollection() {
                    {"index",  "1"},
                    {"row",    "0"},
                    {"action", "download"},
                    {"smt",    "ファイルが作成されました。ここをクリックして下さい。"},
                    /*%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%8C%E4%BD%9C%E6%88%90%E3%81%95%E3%82%8C%E3%81%BE%E3%81%97%E3%81%9F%E3%80%82%E3%81%93%E3%81%93%E3%82%92%E3%82%AF%E3%83%AA%E3%83%83%E3%82%AF%E3%81%97%E3%81%A6%E4%B8%8B%E3%81%95%E3%81%84%E3%80%82*/
                });
            return resData;
        }
    }
}
