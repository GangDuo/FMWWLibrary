using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.MdAnalysis.SalesRanking.Ref
{
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        public event Action Reached;

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override byte[] Excel()
        {
            return base.Excel();
        }

        public override void ExcelAsync()
        {
            base.ExcelAsync();
        }

        public override string Csv()
        {
            return base.Csv();
        }

        public void ReachAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var htmlText = Encoding.UTF8.GetString(args.Result);
                     Debug.WriteLine(htmlText);
                     if (null != Reached)
                     {
                         Reached();
                     }
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, "POST", MainMenuFactory.CreateInstance().Translate());
        }

        public override void Search()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var htmlText = Encoding.UTF8.GetString(args.Result);
                     Debug.WriteLine(htmlText);
                     AjaxForSearch();
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            var url = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("/A210_SALES_RANKING/A210_SELECT.jsp");
            _Client.UploadValuesAsync(url, "POST", this.PageContext.Translate(Context.FormAction.Search));
        }

        private void AjaxForSearch()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     var html = Encoding.UTF8.GetString(args.Result);
                     Debug.WriteLine(html);
                     if (FMWW.Http.Client.IsFin(html))
                     {
                         _Client.UploadValuesCompleted -= onUploadValuesCompleted;

                         FMWW.Http.IPage nextPage = new SearchResults.Page(_Client);
                         OnGoneAway(nextPage);
                         return;
                     }

                     uint row = 0;
                     Match m = Regex.Match(html, @"nextLine\s*=\s*(\d+);", RegexOptions.IgnoreCase);
                     if (m.Success)
                     {
                         row = UInt32.Parse(m.Groups[1].Value);
                     }
                     PostSlipList(row);
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            PostSlipList();
        }

        private void PostSlipList(uint row = 0)
        {
            _Client.UploadValuesAsync(
                UrlSlipList,
                "POST",
                new NameValueCollection() {
                    {"row",      row.ToString()},
                    {"dispMode", "null"},
                    {"index",    "0"},
                    {"cache",    FMWW.Utility.UnixEpochTime.now().ToString()}
                });
        }

        public override void CsvAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var htmlText = Encoding.UTF8.GetString(args.Result);
                     Debug.WriteLine(htmlText);
                     AjaxForCsv();
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(
                FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("/A210_SALES_RANKING/A210_SELECT.jsp"),
                "POST",
                this.PageContext.Translate(Context.FormAction.Export, true)
                );
        }

        private void AjaxForCsv()
        {
            Action execute = () =>
            {
                _Client.UploadValuesAsync(
                    FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/facesAjax/contents/A210_SALES_RANKING/A210_SELECT.jsp"),
                    "POST",
                    new NameValueCollection() {
                    {"form1:isAjaxMode", "1"},
                    {"form1",            "form1"},
                    {"form1:execute",    "execute"},
                    {"cache",            FMWW.Utility.UnixEpochTime.now().ToString()}
                });
            };

            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     var html = Encoding.UTF8.GetString(args.Result);
                     Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss:ff") + " >> " + html);
                     if (FMWW.Http.Client.IsFin(html))
                     {
                         _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                         Download();
                         return;
                     }
                     System.Threading.Thread.Sleep(1000);
                     execute();
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            execute();
        }

        private void Download()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var csvText = Encoding.GetEncoding("Shift_JIS").GetString(args.Result);
                     Debug.WriteLine(csvText);
                     OnCsvDownloadCompleted(csvText);
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(
                FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("/A210_SALES_RANKING/A210_SELECT.jsp"),
                "POST",
                this.PageContext.Translate(Context.FormAction.Export));
        }
    }
}
