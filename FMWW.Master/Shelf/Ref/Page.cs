using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Master.Shelf.Ref
{
    // ﾏｽﾀ -> 棚割ﾏｽﾀ -> 照会
    public class Page : FMWW.Http.Page
    {
        private static readonly Uri address = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X086_160_PROD_SHELF/X086_SELECT.jsp");

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            byte[] resData = this._Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(resData);
            resData = this._Client.UploadValues(address, CreateExportQuery(true));
            html = Encoding.UTF8.GetString(resData);
            FMWW.Core.Helpers.Ajax.Run(this._Client, FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")));
        }

        public override string Csv()
        {
            SignIn();
            Reach();
            var resData = this._Client.UploadValues(address, CreateExportQuery());
            return ShiftJIS.GetString(resData);
        }

        public static List<FMWW.Entity.Shelf> Translate(string csv)
        {
            // parse CSV
            var reader = new StringReader(csv);
            var csvRecords = new List<FMWW.Entity.Shelf>();
            using (var tfp = new Microsoft.VisualBasic.FileIO.TextFieldParser(reader) { Delimiters = new string[] { "," } })
            {
                while (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();
                    csvRecords.Add(new FMWW.Entity.Shelf(fields));
                }
            }
            return csvRecords;
        }

        private static NameValueCollection CreateExportQuery(bool isAjaxMode = false)
        {
            var nvc = new NameValueCollection()
            {
                {"form1:execute",    "execute"},
                {"form1:action",     "export"},
                {"form1:isAjaxMode", ""},
                {"form1",            "form1"},
            };
            if (isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
            }
            return nvc;
        }
    }
}
