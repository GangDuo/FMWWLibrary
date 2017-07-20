using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Mobility.Ordering
{
    public class Page : FMWW.Http.Page
    {
        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        private static readonly Uri UrlX039Servlet = Core.Helpers.UrlBuilder.Build("/JMODE_ASP/X039Servlet");
        private static readonly Uri UrlX039Select = Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X039_TABLET_ORDER/X039_SELECT.jsp");
        private static readonly Uri UrlX039Query = Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X039_TABLET_ORDER/X039_QUERY.jsp");

        private bool IsCancelled { get; set; }
        public override void Cancel()
        {
            this.IsCancelled = true;
        }

        public event LeaveMainMenuForCompletedEventHandler LeaveMainMenuForCompleted;

        protected virtual void OnLeaveMainMenuForCompleted(AsyncCompletedEventArgs e)
        {
            if (LeaveMainMenuForCompleted != null)
                LeaveMainMenuForCompleted(this, e);
        }

        // 商品検索から商品一覧へ遷移
        public void LeaveMainMenuForAsync(Context c, UploadValuesCompletedEventHandler onUploadValuesCompleted)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted02 = null;
            onUploadValuesCompleted02 = (a, args) =>
            {
                this._Client.UploadValuesCompleted -= onUploadValuesCompleted02;

                if (this.IsCancelled)
                {
                    return;
                }
                this._Client.UploadValuesCompleted += onUploadValuesCompleted;
                this._Client.UploadValuesAsync(UrlX039Servlet,
                    new NameValueCollection()
                    {
                        { "m",     "ql"},
                        { "p",     "0"},
                        { "cache", FMWW.Utility.UnixEpochTime.now().ToString()},
                    });
            };

            UploadValuesCompletedEventHandler onUploadValuesCompleted01 = null;
            onUploadValuesCompleted01 = (a, args) =>
            {
                this._Client.UploadValuesCompleted -= onUploadValuesCompleted01;
                if (this.IsCancelled)
                {
                    return;
                }
                AjaxAsync(Core.Helpers.UrlBuilder.Build(UrlX039Select.AbsolutePath.Replace("faces", "facesAjax")),
                    (userState) =>
                    {
                        this._Client.UploadValuesCompleted += onUploadValuesCompleted02;
                        this._Client.UploadValuesAsync(UrlX039Select, c.Translate(Context.FormAction.Preview));
                    });
            };

            if (this.IsCancelled)
            {
                return;
            }
            this._Client.UploadValuesCompleted += onUploadValuesCompleted01;
            this._Client.UploadValuesAsync(UrlX039Select, c.Translate());
        }
        // 商品一覧から商品詳細へ遷移
        public void LeaveSummariesForDetailsAsync(UploadValuesCompletedEventHandler onUploadValuesCompleted, int index = 0)
        {
            Debug.Assert((index >= 0) && (index < 10));

            if (this.IsCancelled)
            {
                return;
            }
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
            this._Client.UploadValuesAsync(UrlX039Servlet,
                new NameValueCollection()
                {
                    { "m",     "qp"},
                    { "i",     index.ToString()},
                    { "cache", FMWW.Utility.UnixEpochTime.now().ToString()},
                });
        }
        // 商品詳細から店舗別在庫一覧へ遷移
        public void LeaveDetailsForStockAsync(Action<int> completed)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (a, args) =>
            {
                // 店舗別在庫一覧
                int q = GetSalesQuantityLastWeek(args);
                if (Core.Helpers.Ajax.IsFin(Encoding.UTF8.GetString(args.Result)))
                {
                    this._Client.UploadValuesCompleted -= onUploadValuesCompleted;
                    completed(q);
                    return;
                }
                RequestAjaxAsync(Core.Helpers.UrlBuilder.Build(UrlX039Query.AbsolutePath.Replace("faces", "facesAjax")), q);
            };

            if (this.IsCancelled)
            {
                return;
            }
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
            this._Client.UploadValuesAsync(UrlX039Query,
                new NameValueCollection()
                {
                    { "form1:action",     "preview"},
                    { "m",                "sh"},
                    { "i",                "0"},
                    { "form1:isAjaxMode", "1"},
                    { "form1",            "form1"},
                    { "form1:execute",    "execute"},
                    { "cache",            FMWW.Utility.UnixEpochTime.now().ToString()},
                });
        }

        private static int GetSalesQuantityLastWeek(UploadValuesCompletedEventArgs args)
        {
            var text = Encoding.UTF8.GetString(args.Result);
            var pattern = @"熊谷肥塚店"";q\.currentQty\s*=\s*\d+;q\.d\s*=\s*\[\];q\.d\[0\]\s*=\s*\d+;q\.d\[1\]\s*=\s*\d+;q\.d\[2\]\s*=\s*\d+;q\.d\[3\]\s*=\s*(\d+);";
            int q = (int)(args.UserState ?? 0);
            if (Regex.IsMatch(text, pattern))
            {
                var m = Regex.Match(text, pattern);
                q = Int32.Parse(m.Groups[1].Value);
            }
            return q;
        }
    }
}
