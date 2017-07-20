using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using T = Text;

namespace FMWW.Inventory.Archive.Book.Ref
{
    // 在庫・棚卸[在庫] -> 在庫照会 -> 照会
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }
        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override byte[] Excel()
        {
            return base.Excel();
        }

        public override string Csv()
        {
            SignIn();
            // メインメニュー 在庫照会->照会
            byte[] resData = this._Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var _html = ShiftJIS.GetString(resData);

            // 在庫検索
            var nvc = PageContext.Translate();
            List<string> aa = new List<string>();
            foreach (string key in nvc.Keys)
            {
                var enc = Encoding.GetEncoding("shift-jis");
                var editor = new StringBuilder();
                editor.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, enc), HttpUtility.UrlEncode(nvc[key], enc));
                aa.Add(editor.ToString());
            }
            var query_ = Encoding.ASCII.GetBytes(String.Join("&", aa));
            this._Client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            resData = this._Client.UploadData(FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/S300_STOCK/S300_SELECT.jsp"), query_);
            _html = Encoding.GetEncoding("shift-jis").GetString(resData);

            // 在庫一覧
            this._Client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            resData = this._Client.UploadData(FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/SlipList2Servlet"), Encoding.ASCII.GetBytes("mode=0&cache=" + FMWW.Core.Helpers.Ajax.TimeStamp()));
            _html = Encoding.UTF8.GetString(resData);
            if (Regex.IsMatch(_html, "isError[^=]*=[^;]*1;"))
            {
                // サーバエラー
                throw new Exception("サーバエラー");
            }
            if (Regex.IsMatch(_html, @"maxPage[^=]*=[^\d]*0;"))
            {
                // 表示するデータがありません。
                throw new Exception("表示するデータがありません。");
            }

            // CSV出力
            string type = "cvs";//"excel"
            this._Client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            resData = this._Client.UploadData(FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/Export"),
                Encoding.ASCII.GetBytes("type=" + type + "&mode=start&cache=" + FMWW.Core.Helpers.Ajax.TimeStamp()));
            _html = Encoding.UTF8.GetString(resData);

            while (!FMWW.Core.Helpers.Ajax.IsFin(_html))
            {
                this._Client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                resData = this._Client.UploadData(FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/Export"),
                    Encoding.ASCII.GetBytes("mode=check&cache=" + FMWW.Core.Helpers.Ajax.TimeStamp()));
                _html = Encoding.UTF8.GetString(resData);
                System.Threading.Thread.Sleep(1000);
            }

            // CSVファイルダウンロード
            //var fname = String.Format(@"{0}_{1}_{2}.csv",
            //    filter.ShopCode,
            //    ((filter.LineCodes.Length > 0) ?
            //        String.Join("-", filter.LineCodes) :
            //        ((filter.SupplierCode.Length > 0) ?
            //            filter.SupplierCode :
            //            ((filter.ItemCodes.Length > 0) ?
            //                String.Join("-", filter.ItemCodes) :
            //                ""))),
            //    filter.Date.ToString("d"));
            var ms = new MemoryStream();
            var ub = new UriBuilder(Uri.UriSchemeHttps, FMWW.Core.AbstractAuthentication.HostName)
            {
                Path = "/JMODE_ASP/Export",
                Query = "mode=download&cache=" + FMWW.Utility.UnixEpochTime.now()
            };
            var url = ub.Uri.ToString();
            using (var st = this._Client.OpenRead(url))
            {
                if ("gzip" == this._Client.ResponseHeaders["Content-Encoding"])
                {
                    int num;
                    byte[] buf = new byte[1024]; // 1Kbytesずつ処理する
                    var decompStream // 解凍ストリーム
                       = new GZipStream(
                         st, // 入力元となるストリームを指定
                         CompressionMode.Decompress); // 解凍（圧縮解除）を指定
                    using (decompStream)
                    {
                        while ((num = decompStream.Read(buf, 0, buf.Length)) > 0)
                        {
                            ms.Write(buf, 0, num);
                        }
                    }
                }
                else
                {
                    int b;
                    while ((b = st.ReadByte()) != -1)
                    {
                        ms.WriteByte((byte)b);
                    }
                }
                st.Close();
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        // Stock CSV -> DataTable
        public static DataTable StockCSV2DataTable(MemoryStream st)
        {
            var table = FMWW.Entity.Stock.CreateSimpleTable();
            var colIndexes = new Dictionary<string, int>(){
                    {"sku", 16},
                    {"store_code", 0},
                    {"quantity", 18},
                };
            //T.Csv.Convert(Encoding.UTF8.GetString(buf), table, colIndexes);
            st.Seek(0, SeekOrigin.Begin);
            T.Csv.Convert(st, table, colIndexes);
            return table;
        }
    }
}
