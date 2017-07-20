using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.MdAnalysis.SalesRanking.Ref.SearchResults
{
    class Page : FMWW.Http.Page
    {
        public Page(FMWW.Http.Client client) : base(client) { }

        public override void ExcelAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var htmlText = Encoding.UTF8.GetString(args.Result);
                     Debug.Assert(FMWW.Http.Client.IsFin(htmlText));
                     DownloadAsync();
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(
                UrlSlipList,
                "POST",
                new NameValueCollection() {
                    {"action", "excel"},
                    {"row",    "0"},
                    {"index",  "0"},
                    {"cache",  FMWW.Utility.UnixEpochTime.now().ToString()}
                });
        }

        private void DownloadAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     OnExcelDownloadCompleted(args.Result);
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(
                UrlSlipList,
                "POST",
                new NameValueCollection() {
                    {"index",  "0"},
                    {"row",    "0"},
                    {"action", "download"},
                    {"smt",    "ファイルが作成されました。ここをクリックして下さい。"}
                });
        }
    }
}
