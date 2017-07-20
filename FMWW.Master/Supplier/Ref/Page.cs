using FMWW.Master.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Master.Supplier.Ref
{
    // ﾏｽﾀ[各種ﾏｽﾀ] -> 仕入先ﾏｽﾀ -> 照会
    public class Page : FMWW.Http.Page
    {
        private Dictionary<string, string> Supplier;

        public Page() : base()
        {
            PageContext = new Context();
        }
        public Page(FMWW.Http.Client client) : base(client) { }

        public Context PageContext { get; set; }

        private event ExportSupplierCompletedEventHandler ExportSupplierCompleted;
        private event CountSuppliersCompletedEventHandler CountSuppliersCompleted;
        
        internal virtual void OnExportSupplierCompleted(ExportSupplierCompletedEventArgs e)
        {
            if (ExportSupplierCompleted != null)
                ExportSupplierCompleted(this, e);
        }
        internal virtual void OnCountSuppliersCompleted(CountSuppliersCompletedEventArgs e)
        {
            if (CountSuppliersCompleted != null)
                CountSuppliersCompleted(this, e);
        }

        public void CountSuppliersAsync(object userToken = default(object))
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, string> Parse(string html)
        {
            var supplier = new Dictionary<string, string>();
            var parser = new FMWW.Http.HTMLParser(html);
            mshtml.HTMLDocument document = parser.Document;
            mshtml.IHTMLElementCollection inputs = document.getElementsByTagName("input");
            foreach (mshtml.IHTMLElement elm in inputs)
            {
                var title = elm.getAttribute("title") ?? "";
                var id = elm.id ?? "";
                if ((title.Length == 0) || (id.Length == 0))
                {
                    continue;
                }
                var value = elm.getAttribute("value");
                supplier.Add(title, value);
                Trace.WriteLine(String.Format("{0} {1} {2}", elm.id, title, value));

                try
                {
                    var input = elm as mshtml.IHTMLInputElement;
                    var suffix = ":select";
                    var t = title + suffix;
                    var v = "";
                    mshtml.IHTMLElementCollection spans = document.getElementById(elm.id + suffix).children;
                    foreach (mshtml.IHTMLElement span in spans)
                    {
                        if (span.getAttribute("value") == input.value)
                        {
                            v = span.getAttribute("text");
                            continue;
                        }
                    }
                    supplier.Add(t, v);
                    Trace.WriteLine(String.Format("{0} {1} {2}", elm.id + suffix, t, v));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
            mshtml.IHTMLElement remark = document.getElementById("remark");
            supplier.Add("備考", remark.innerText);
            return supplier;
        }

        public override string Csv()
        {
            var suppliers = new List<Dictionary<string, string>>();
            SignIn();
            this._Client.UploadValues(FMWW.Core.MainMenu.Url, FMWW.Http.Method.Post, MainMenuFactory.CreateInstance().Translate());

            var address = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/M040_SUPPLIER/M040_SELECT.jsp");
            var data = PageContext.Translate();
            uint row = 1;
            var clickedRow = 1;
            while (row >= clickedRow)
            {
                row = LoadSummary(address, data);

                var resData = this._Client.UploadValues(FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/M040_SUPPLIER/M040_LIST.jsp"),
                    new NameValueCollection()
                                {
                                    {"form1:execute",    "execute"},
                                    {"form1:action",     "search"},
                                    {"form1:isAjaxMode", ""},
                                    {"form1:clickRow",   clickedRow.ToString()},
                                    {"form1",            "form1"},
                                });
                ++clickedRow;

                // 仕入先情報保存
                var supplier = Parse(Encoding.UTF8.GetString(resData));
                suppliers.Add(supplier);
                foreach (var key in supplier.Keys)
                {
                    Trace.WriteLine(String.Format("{0}: {1}", key, supplier[key]));
                }

                // 前画面に戻る
                address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("M040_SUPPLIER/M040_QUERY.jsp");
                data = new NameValueCollection()
                {
                    {"form1:execute",    "execute"},
                    {"form1:action",     "quit"},
                    {"form1:isAjaxMode", ""},
                    {"form1",            "form1"},
                };
            }


            return base.Csv();
        }

        private uint LoadSummary(Uri address, NameValueCollection data)
        {
            byte[] resData = this._Client.UploadValues(address, data);

            //FMWW.Core.Helpers.Ajax.Run(this._Client, FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")));
            uint row = 0;
            var text = Encoding.UTF8.GetString(resData);
            while (!FMWW.Core.Helpers.Ajax.IsFin(text))
            {
                row = HighlightNextLine(text);
                resData = this._Client.UploadValues(UrlSlipList, new NameValueCollection()
                            {
                                {"row",      row.ToString()},
                                {"dispMode", "null"},
                                {"index",    "0"},
                                {"cache",    FMWW.Utility.UnixEpochTime.now().ToString()},
                            });
                text = Encoding.UTF8.GetString(resData);
                if (FMWW.Core.Helpers.Ajax.HasError(text))
                {
                    throw new Exception();
                }
            }
            row = HighlightNextLine(text);
            return row;
        }

        private static uint HighlightNextLine(string text)
        {
            uint row = 0;
            Match m = Regex.Match(text, @"nextLine\s*=\s*(\d+);", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                row = UInt32.Parse(m.Groups[1].Value);
            }
            return row;
        }
    }
}
