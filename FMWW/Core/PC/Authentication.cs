using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.Core.PC
{
    interface IAuthenticatable
    {
        void SignIn(FMWW.Http.Client client, string username, string password, string person, string personPassword);
        void SignInAsync(FMWW.Http.Client client, string username, string password, string person, string personPassword, object userToken = default(object));
    }

    public class Authentication : AbstractAuthentication, IAuthenticatable
    {
        private static readonly string[] ClientQueryNames = new string[] { "form1:client", "form1:username" };
        private static readonly string[] PersonQueryNames = new string[] { "form1:person_code", "form1:person" };
        private static readonly string[] PasswordQueryNames = new string[] { "form1:password", "form1:clpass" };
        private static readonly string[] PersonPasswordQueryNames = new string[] { "form1:person_password", "form1:pspass" };

        private static readonly Uri UrlToSignIn = Helpers.UrlBuilder.Build("/JMODE_ASP/faces/login.jsp");

        private static mshtml.IHTMLElement GetBtnLogOff(string html)
        {
            return (new FMWW.Http.HTMLParser(html)).Document.getElementById("log_off");
        }

        public static NameValueCollection BuildQueryToSignIn(string html, string username, string password, string person, string personPassword)
        {
            var inputs = (new FMWW.Http.HTMLParser(html)).Document.getElementsByTagName("input");
            var form = new NameValueCollection();
            foreach (mshtml.IHTMLInputElement input in inputs)
            {
                if (null == input.name)
                {
                    continue;
                }
                var name = input.name.ToString();
                var value = "";
                Debug.WriteLine(name);
                if (IsClientQueryName(name))
                {
                    value = username;
                }
                else if (IsPersonQueryName(name))
                {
                    value = person;
                }
                else if (IsPasswordQueryName(name))
                {
                    value = password;
                }
                else if (IsPersonPasswordQueryName(name))
                {
                    value = personPassword;
                }
                else
                {
                    value = input.value.ToString();
                }
                form.Add(name, value);
            }
            return form;
        }

        public void SignIn(FMWW.Http.Client client, string username, string password, string person, string personPassword)
        {
            //            this.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            Stream data = client.OpenRead(UrlToSignIn);
            StreamReader reader = new StreamReader(data, Encoding.UTF8);
            var html = reader.ReadToEnd();
            data.Close();
            reader.Close();

#if false
            //            var setCookie = this.ResponseHeaders[HttpResponseHeader.SetCookie];
            //            var cookies = System.Text.RegularExpressions.Regex.Split(setCookie, "(?<!expires=.{3}),")
            //    .Select(s => s.Split(';').First().Split('='))
            //    .Select(xs => new { Name = xs.First(), Value = string.Join("=", xs.Skip(1).ToArray()) })
            //    .Select(a => a.Name + "=" + a.Value)
            //    .ToArray();
            //            var cookie = string.Join(";", cookies);
            //            this.Headers[HttpRequestHeader.Cookie] = cookie;
#endif


            NameValueCollection form = BuildQueryToSignIn(html, username, password, person, personPassword);

            //            this.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            byte[] resData = client.UploadValues(UrlToSignIn, form);
            //            _html = System.Text.Encoding.GetEncoding("Shift_JIS").GetString(resData);
            html = Encoding.UTF8.GetString(resData);
            Debug.WriteLine(html);
            if (null == GetBtnLogOff(html))
            {
                throw new Exception();
            }

            /*
                        this.OpenReadCompleted += (sender, e) =>
                            {
                                if (e.Error != null)
                                {
                                    // エラー処理
                                }

                                var data = e.Result;
                                // Do Something
                                if (!(data is System.IO.Stream))
                                {
                                    return;
                                }
                                if (!(sender is WebClient)) 
                                {
                                    return;
                                }
            //                    WebClient wc = sender as WebClient;

                                foreach (string key in client.ResponseHeaders.Keys)
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format(@"{0}:{1}", key, client.ResponseHeaders[key]));
                                }

                            };
             * */
        }

        public void SignInAsync(FMWW.Http.Client client, string username, string password, string person, string personPassword, object userToken = default(object))
        {
            var onUploadValuesCompleted = new Action<object, UploadValuesCompletedEventArgs>(
                (o, args) =>
                {
                    client.UploadValuesCompleted -= new UploadValuesCompletedEventHandler((Action<object, UploadValuesCompletedEventArgs>)args.UserState);
                    var html = Encoding.UTF8.GetString(args.Result);
                    var a = new SignedInEventArgs(args.Error, args.Cancelled, userToken);
                    //completedEventHandler(html);
                    if (null == GetBtnLogOff(html))
                    {
                        a = new SignedInEventArgs(new Exception(), args.Cancelled, userToken);
                    }
                    // ログイン成功
                    OnSignedIn(a);
                });

            var onOpenReadCompleted = new Action<object, OpenReadCompletedEventArgs>(
                (o, args) =>
                {
                    client.OpenReadCompleted -= new OpenReadCompletedEventHandler((Action<object, OpenReadCompletedEventArgs>)args.UserState);
                    Stream data = args.Result;
                    StreamReader reader = new StreamReader(data, Encoding.UTF8);
                    var html = reader.ReadToEnd();
                    data.Close();
                    reader.Close();

                    client.UploadValuesAsync(UrlToSignIn, Http.Method.Post,
                        BuildQueryToSignIn(html, username, password, person, personPassword),
                        onUploadValuesCompleted);
                });

            client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(onUploadValuesCompleted);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(onOpenReadCompleted);
            client.OpenReadAsync(UrlToSignIn, onOpenReadCompleted);
        }

        public static bool CanClickLogOff(string html)
        {
            return null != GetBtnLogOff(html);
        }

        public static bool IsClientQueryName(string name)
        {
            return ClientQueryNames.Contains(name);
        }
        public static bool IsPersonQueryName(string name)
        {
            return PersonQueryNames.Contains(name);
        }
        public static bool IsPasswordQueryName(string name)
        {
            return PasswordQueryNames.Contains(name);
        }
        public static bool IsPersonPasswordQueryName(string name)
        {
            return PersonPasswordQueryNames.Contains(name);
        }
    }
}
