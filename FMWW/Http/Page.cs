using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.Http
{
    public abstract class Page : IPage
    {
        protected static readonly Uri UrlSlipList = Core.Helpers.UrlBuilder.Build("/JMODE_ASP/SlipList");
        protected static readonly Encoding ShiftJIS = Encoding.GetEncoding("Shift_JIS");

        public Entity.UserAccount UserAccount { get; set; }

        public event Action<IPage> GoneAway;
        public event Action<string> CsvDownloadCompleted;
        public event Action<byte[]> ExcelDownloadCompleted;

        protected Client _Client;
        protected Core.PC.Authentication Auth = new Core.PC.Authentication();

        public Page() : this(new Client() { CookieContainer = new CookieContainer() }) { }

        public Page(Client client)
        {
            this._Client = client;
        }

        protected void OnGoneAway(IPage nextPage)
        {
            if (null != GoneAway)
            {
                GoneAway(nextPage);
            }
        }
        protected void OnCsvDownloadCompleted(string result)
        {
            if (null != CsvDownloadCompleted)
            {
                CsvDownloadCompleted(result);
            }
        }
        protected void OnExcelDownloadCompleted(byte[] result)
        {
            if (null != ExcelDownloadCompleted)
            {
                ExcelDownloadCompleted(result);
            }
        }

        protected void RequestAjaxAsync(Uri address, object userState = null)
        {
            this._Client.UploadValuesAsync(address, FMWW.Http.Method.Post, Core.Helpers.Ajax.CreateAjaxQuery(), userState);
        }

        protected void AjaxAsync(Uri address, Action<object> completed, object userState = null)
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = (o, args) =>
                {
                    var html = Encoding.UTF8.GetString(args.Result);
                    if (Core.Helpers.Ajax.IsFin(html))
                    {
                        this._Client.UploadValuesCompleted -= onUploadValuesCompleted;
                        completed(args.UserState);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                        RequestAjaxAsync(address, userState);
                    }
                };
            this._Client.UploadValuesCompleted += onUploadValuesCompleted;
            RequestAjaxAsync(address, userState);
        }

        private static string GetPageTitle(string html)
        {
#if true
            return (new HTMLParser(html)).Document.title;
#else
            foreach (mshtml.IHTMLElement elm in (new HTMLParser(html)).Document.getElementsByTagName("title"))
            {
                return elm.innerHTML;
            }
            return "";
#endif
        }

        protected void SignIn()
        {
            Auth.SignIn(this._Client, UserAccount.UserName, UserAccount.Password, UserAccount.Person, UserAccount.PersonPassword);
        }

        protected void SignInAsync(object userToken = default(object))
        {
            Auth.SignInAsync(this._Client, UserAccount.UserName, UserAccount.Password, UserAccount.Person, UserAccount.PersonPassword, userToken);
        }

        public virtual void Quit()
        {
            throw new NotImplementedException();
        }

        public virtual void Register()
        {
            throw new NotImplementedException();
        }

        public virtual void Search()
        {
            throw new NotImplementedException();
        }

        public virtual void Del()
        {
            throw new NotImplementedException();
        }

        public virtual void Print()
        {
            throw new NotImplementedException();
        }

        public virtual void CsvAsync()
        {
            throw new NotImplementedException();
        }

        public virtual string Csv()
        {
            throw new NotImplementedException();
        }

        public virtual void ExcelAsync()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] Excel()
        {
            throw new NotImplementedException();
        }

        public virtual void Preview()
        {
            throw new NotImplementedException();
        }

        public virtual void Check()
        {
            throw new NotImplementedException();
        }

        public virtual void Cancel()
        {
            throw new NotImplementedException();
        }

        public virtual void Xqt()
        {
            throw new NotImplementedException();
        }
    }
}
