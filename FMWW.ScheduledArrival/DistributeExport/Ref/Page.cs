using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FMWW.ScheduledArrival.DistributeExport.Ref
{
    // 入荷予定 -> 投入表ｴｸｽﾎﾟｰﾄ -> 照会
    public class Page : FMWW.Http.Page
    {
        private static readonly string SchemeName = Uri.UriSchemeHttps;
        private static readonly Uri UrlX024Select = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp");
        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            var context = this.PageContext ?? new Context();
            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(resData);

            resData = _Client.UploadValues(
                FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp"),
                context.Translate(true));
            html = Encoding.UTF8.GetString(resData);

            FMWW.Core.Helpers.Ajax.Run(this._Client, UrlX024Select);
        }

        private void ReachWebPageDistributeExportAsync(Action<string> completed)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
                {
                    this._Client.UploadValuesCompleted -= onUploadValuesCompleted;
                    var html = Encoding.UTF8.GetString(args.Result);
                    completed(html);
                };
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
            this._Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, FMWW.Http.Method.Post, MainMenuFactory.CreateInstance().Translate());
        }

        // 入荷予定 -> 投入表ｴｸｽﾎﾟｰﾄ 条件を入力し絞込
        private void FilterDistributeExportAsync(Context context, Action<string> completed)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
                {
                    this._Client.UploadValuesCompleted -= onUploadValuesCompleted;
                    var html = Encoding.UTF8.GetString(args.Result);
                    completed(html);
                };

            this._Client.UploadValuesAsync(
                new UriBuilder(SchemeName, FMWW.Core.Config.Instance.HostName) { Path = "/JMODE_ASP/faces/contents/X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp" }.Uri,
                FMWW.Http.Method.Post, context.Translate(true));
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
        }

        private void DownloadDistributeExportWithCsvAsync(Context context, Action<string> completed)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
                {
                    this._Client.UploadValuesCompleted -= onUploadValuesCompleted;
                    var csv = ShiftJIS.GetString(args.Result);
                    completed(csv);
                };

            this._Client.UploadValuesAsync(
                FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp"),
                FMWW.Http.Method.Post, context.Translate());
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
        }

        public override void CsvAsync()
        {
            Action<string> onDownloadDistributeExportWithCsv = (csv) =>
            {
                OnCsvDownloadCompleted(csv);
            };

            Action<object> onAjax = (userState) =>
            {
                DownloadDistributeExportWithCsvAsync(PageContext, onDownloadDistributeExportWithCsv);
            };

            Action<string> onFilterDistributeExport = (html) =>
             {
                 AjaxAsync(
                     FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/facesAjax/contents/X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp"),
                     onAjax);
             };

            Action<string> onReachWebPageDistributeExport = (html) =>
            {
                FilterDistributeExportAsync(PageContext, onFilterDistributeExport);
            };

            FMWW.Component.SignedInEventHandler onSignedIn = null;
            onSignedIn = (a, b) =>
            {
                Auth.SignedIn -= onSignedIn;
                ReachWebPageDistributeExportAsync(onReachWebPageDistributeExport);
            };
            Auth.SignedIn += onSignedIn;
            SignInAsync();
        }

        public override string Csv()
        {
            SignIn();
            Reach();

            var context = this.PageContext ?? new Context();
            var resData = _Client.UploadValues(UrlX024Select, context.Translate());
            var html = ShiftJIS.GetString(resData);
            Debug.WriteLine(html);
            return html;
        }

        public static string GetShopArrivalDate(FMWW.Http.Client client, string distribute)
        {
            var page = new Page(client)
            {
                PageContext = new FMWW.ScheduledArrival.DistributeExport.Ref.Context()
                {
                    Code = distribute
                }
            };
            page.Reach();
            var html = page.Csv();
            var fields = html.Split(new char[] { '\r', '\n' }).First().Split(',').ToArray();
            string shopArrivalDate = fields[3].Replace('-', '/');
            Debug.WriteLine(shopArrivalDate);
            return shopArrivalDate;
        }
    }
}
