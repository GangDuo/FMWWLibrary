using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.ExternalInterface.Products.Ref
{
    // 外部ｲﾝﾀｰﾌｪｲｽ -> 商品ﾏｽﾀﾒﾝﾃﾅﾝｽ -> 照会
    public class Page : FMWW.Http.Page
    {
        public Context PageContext { get; set; }

        private static readonly Uri Address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X021_080_PRODUCT_EXPORT/X021_SELECT.jsp");

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void Reach()
        {
            byte[] resData = this._Client.UploadValues(FMWW.Core.MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(resData);
            this._Client.UploadValues(Address, this.PageContext.Translate(true));
            FMWW.Core.Helpers.Ajax.Run(this._Client, FMWW.Core.Helpers.UrlBuilder.Build(Address.AbsolutePath.Replace("faces", "facesAjax")));
        }
        
        public void ReachAsync(Action completed)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted2 = null;
            onUploadValuesCompleted2 = (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted2;
                     AjaxAsync(FMWW.Core.Helpers.UrlBuilder.Build(Address.AbsolutePath.Replace("faces", "facesAjax")),
                         (userState) =>
                         {
                             completed();
                         });                    
                 };

            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     _Client.UploadValuesCompleted += onUploadValuesCompleted2;
                     var url = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X021_080_PRODUCT_EXPORT/X021_SELECT.jsp");
                     _Client.UploadValuesAsync(url, FMWW.Http.Method.Post, PageContext.Translate(true));
                 };
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, FMWW.Http.Method.Post, MainMenuFactory.CreateInstance().Translate());
        }

        public override void Quit()
        {
            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X021_080_PRODUCT_EXPORT/X021_SELECT.jsp");
            var c = new Context() { FormAction = "quit" };
            _Client.UploadValues(address, c.Translate());
        }

        public override byte[] Excel()
        {
            SignIn();
            Reach();

            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X021_080_PRODUCT_EXPORT/X021_SELECT.jsp");
            //_Client.UploadValues(address, c.Translate(true));
            //_Client.AjaxForSearch(Core.FMWW.Client.BuildUrl(address.AbsolutePath.Replace("faces", "facesAjax")));
            return _Client.UploadValues(address, PageContext.Translate());
        }

        public override void ExcelAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (x, y) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     OnExcelDownloadCompleted(y.Result);
                 };

            SignedInEventHandler onSignedIn = null;
            onSignedIn = (a, b) =>
            {
                Auth.SignedIn -= onSignedIn;
                ReachAsync(() =>
                {
                    _Client.UploadValuesCompleted += onUploadValuesCompleted;
                    _Client.UploadValuesAsync(Address, FMWW.Http.Method.Post, PageContext.Translate());
                });
            };
            Auth.SignedIn += onSignedIn;
            SignInAsync();
        }

        [Obsolete]
        public void DownloadProductMasterAsync(Context c, Action<byte[], Exception, object> DownloadProductMasterCompleted, object userToken = default(object))
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = (x, y) =>
            {
                if (null != y.Error)
                {
                    DownloadProductMasterCompleted(y.Result, y.Error, userToken);
                    return;
                }
                var userState = (Tuple<Context, object, UploadValuesCompletedEventHandler, string>)y.UserState;
                if (userState.Item4 == FMWW.Core.MainMenu.Url.ToString())
                {


                    var url = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X021_080_PRODUCT_EXPORT/X021_SELECT.jsp").ToString();
                    this._Client.UploadValuesAsync(new Uri(url), FMWW.Http.Method.Post, c.Translate(true),
                        Tuple.Create(userState.Item1, userState.Item2, userState.Item3, url));
                }
                else if (userState.Item4 == FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X021_080_PRODUCT_EXPORT/X021_SELECT.jsp").ToString()
                      || userState.Item4 == FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/facesAjax/contents/X021_080_PRODUCT_EXPORT/X021_SELECT.jsp").ToString())
                {
                    var html = Encoding.UTF8.GetString(y.Result);
                    if (FMWW.Core.Helpers.Ajax.HasError(html))
                    {
                        throw new Exception();
                    }
                    var url = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/facesAjax/contents/X021_080_PRODUCT_EXPORT/X021_SELECT.jsp").ToString();
                    var url2 = url;
                    var data = FMWW.Core.Helpers.Ajax.CreateAjaxQuery();
                    if (FMWW.Core.Helpers.Ajax.IsFin(html))
                    {
                        url = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/X021_080_PRODUCT_EXPORT/X021_SELECT.jsp").ToString();
                        data = c.Translate();
                        url2 = "";
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                    this._Client.UploadValuesAsync(new Uri(url), FMWW.Http.Method.Post, data,
                        Tuple.Create(userState.Item1, userState.Item2, userState.Item3, url2));
                }
                else
                {
                    this._Client.UploadValuesCompleted -= userState.Item3;
                    DownloadProductMasterCompleted(y.Result, null, userToken);

                }
            };

            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
            this._Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, FMWW.Http.Method.Post,
                MainMenuFactory.CreateInstance().Translate(),
                Tuple.Create(c, userToken, onUploadValuesCompleted, FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/index.jsp").ToString()));
        }
    }
}
