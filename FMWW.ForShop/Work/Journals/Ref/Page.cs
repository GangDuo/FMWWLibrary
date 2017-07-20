using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.ForShop.Work.Journals.Ref
{
    // 店舗管理 [店舗業務] -> 店舗売上入力リスト -> 照会
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }
        public event Action<string> Reached;

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
#if DEBUG
            var html = Encoding.UTF8.GetString(resData);
            Debug.WriteLine(html);
#endif
        }

        public void ReachAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
            {
                _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                var html = Encoding.UTF8.GetString(args.Result);
                if (null != Reached)
                {
                    Reached(html);
                }
            };
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
        }

        public override byte[] Excel()
        {
            return base.Excel();
        }

        public override void ExcelAsync()
        {
            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("S086_SHOP_SALES_REPORT/S086_SELECT.jsp");

            #region OnAjax
            Action<object> OnAjax = null;
            OnAjax = (userState) =>
            {
                #region onExcelDownloaded
                UploadValuesCompletedEventHandler onExcelDownloaded = null;
                onExcelDownloaded = (object o, UploadValuesCompletedEventArgs arg) =>
                {
                    _Client.UploadValuesCompleted -= onExcelDownloaded;
                    OnExcelDownloadCompleted(arg.Result);
                };
                #endregion
                _Client.UploadValuesCompleted += onExcelDownloaded;
                _Client.UploadValuesAsync(address, PageContext.Translate());
            };
            #endregion

            #region onUploadValuesCompleted
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (object o, UploadValuesCompletedEventArgs arg) =>
            {
                _Client.UploadValuesCompleted -= onUploadValuesCompleted;

                AjaxAsync(
                    FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")),
                    OnAjax);
            };
            #endregion

            Action<string> OnReached = null;
            OnReached = (string html) =>
            {
                Reached -= OnReached;
                _Client.UploadValuesCompleted += onUploadValuesCompleted;
                _Client.UploadValuesAsync(address, PageContext.Translate(true));
            };

            #region onSignedIn
            FMWW.Component.SignedInEventHandler onSignedIn = null;
            onSignedIn = (o, arg) =>
            {
                Auth.SignedIn -= onSignedIn;
                Reached += OnReached;
                ReachAsync();
            };
            #endregion

            Auth.SignedIn += onSignedIn;
            SignInAsync();

        }
    }
}
