using FMWW.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.Core.Mobile
{
    interface IAuthenticatable
    {
        void SignIn(FMWW.Http.Client client, string person, string personPassword);
        void SignInAsync(FMWW.Http.Client client, string person, string personPassword, object userToken = default(object));
    }

    class Authentication : AbstractAuthentication, IAuthenticatable
    {
        private static readonly string[] ClientQueryNames = new string[] { "form1:client", "form1:username" };
        private static readonly string[] PasswordQueryNames = new string[] { "form1:password", "form1:clpass" };

        public void SignIn(FMWW.Http.Client client, string person, string personPassword)
        {
            throw new NotImplementedException();
        }

        private static readonly Uri UrlToSignIn = Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/tabletLogin.jsp");

        public void SignInAsync(FMWW.Http.Client client, string person, string personPassword, object userToken = default(object))
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted1 = null;
            onUploadValuesCompleted1 = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     client.UploadValuesCompleted -= onUploadValuesCompleted1;
                     var html = Encoding.UTF8.GetString(args.Result);
                     //if (null != GetBtnLogOff(html))
                     {
                         // ログイン成功
                         OnSignedIn(new SignedInEventArgs(args.Error, args.Cancelled, userToken));
                     }
                 });

            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var html = Encoding.UTF8.GetString(args.Result);
                     if (!Core.PC.Authentication.CanClickLogOff(html))
                     {
                         // ログイン失敗
                         return;
                     }
                     // ログイン成功
                     client.UploadValuesCompleted += onUploadValuesCompleted1;
                     client.UploadValuesAsync(MainMenu.Url, MainMenuFactory.CreateInstance().Translate());
                 });

            OpenReadCompletedEventHandler onOpenReadCompleted = null;
            onOpenReadCompleted = new OpenReadCompletedEventHandler(
                 (o, args) =>
                 {
                     client.OpenReadCompleted -= onOpenReadCompleted;

                     using (var reader = new StreamReader(args.Result, ShiftJIS))
                     {
                         var html = reader.ReadToEnd();
                         #region HTMLに埋め込まれた、ユーザ名とパスワードを取得
                         Func<string, string, string> f = (ax, id) =>
                         {
                             if (!String.IsNullOrEmpty(ax))
                             {
                                 return ax;
                             }
                             var input = (new FMWW.Http.HTMLParser(html)).Document.getElementById(id);
                             if (input == null)
                             {
                                 return ax;
                             }
                             return input.getAttribute("value");
                         };
                         var userName = ClientQueryNames.Aggregate(String.Empty, f);
                         var password = PasswordQueryNames.Aggregate(String.Empty, f);
                         #endregion
                         client.UploadValuesCompleted += onUploadValuesCompleted;
                         client.UploadValuesAsync(UrlToSignIn, Core.PC.Authentication.BuildQueryToSignIn(html, userName, password, person, personPassword));
                     }
                 });

            client.OpenReadCompleted += onOpenReadCompleted;
            client.OpenReadAsync(UrlToSignIn, onOpenReadCompleted);
        }
    }
}
