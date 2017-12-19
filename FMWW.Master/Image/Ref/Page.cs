using FMWW.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Master.Image.Ref
{
    public class Page : Http.Page
    {
        public Page() : base() { }
        public Page(Http.Client client) : base(client) { }

        public Stream AsStreamBy(string code)
        {
            SignIn();
            return _Client.OpenRead(Core.Helpers.UrlBuilder.GetImageUrlBy(code));
        }

        public void DownloadBy(string code, string fileName)
        {
            SignIn();
            this._Client.DownloadFile(
                        Core.Helpers.UrlBuilder.GetImageUrlBy(code),
                        fileName);
//                        String.Format(@"{0}\{1}.jpg", context.WorkDir, code));
        }

        public void DownloadAsync()
        {
            SignedInEventHandler onSignedIn = null;
            onSignedIn = (a, b) =>
            {
                Auth.SignedIn -= onSignedIn;
                // TODO:非同期で画像ダウンロード
            };
            Auth.SignedIn += onSignedIn;
            SignInAsync();
        }
    }
}
