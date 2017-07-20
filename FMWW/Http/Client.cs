using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using System.IO.Compression;
using System.Data;
using T = Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace FMWW.Http
{
    public class Client : WebClient
    {
        private static readonly string _UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; MDDRJS)";
        protected static readonly Encoding ShiftJIS = Encoding.GetEncoding("Shift_JIS");
        private static readonly string SchemeName = Uri.UriSchemeHttps;

        private static readonly Uri UrlX024Select = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("X024_160_DISTRIBUTE_EXPORT/X024_SELECT.jsp");

        public CookieContainer CookieContainer { get; set; }
        public string UserAgent { get; set; }

        /**
           --1234567890
           Content-Disposition: form-data; name="フォーム名1"; filename="ファイル名1"
           Content-Type: application/octet-stream
           Content-Transfer-Encoding: binary

           (ここにファイル内容)
           --1234567890
           Content-Disposition: form-data; name="フォーム名2"; filename="ファイル名2"
           Content-Type: application/octet-stream
           Content-Transfer-Encoding: binary

           (ここにファイル内容)
           --1234567890
           Content-Disposition: form-data; name="フォーム名3"; filename="ファイル名3"
        */
        public void UploadFiles(string url, List<string> files)
        {
            var tick = Environment.TickCount.ToString();
            var enc = Encoding.UTF8;

            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + tick;
            if (req is HttpWebRequest)
            {
                (req as HttpWebRequest).CookieContainer = this.CookieContainer;
            }

            byte[] boundary = enc.GetBytes("--" + tick);
            byte[] crlf = enc.GetBytes("\r\n");
            List<byte[]> headers = new List<byte[]>();

            //ヘッダの作成とデータサイズの計算
            long contentLen = 0;
            for (int i = 0; i < files.Count; i++)
            {
                //ヘッダ
                string header = "Content-Disposition: form-data; name=\"upfile" + i.ToString() + "\"; filename=\"" + Path.GetFileName(files[i]) + "\"\r\n" +
                                "Content-Type: application/octet-stream\r\n" +
                                "Content-Transfer-Encoding: binary\r\n\r\n";

                headers.Add(enc.GetBytes(header));

                //1ファイルごとのデータサイズ
                contentLen += headers[i].Length + new FileInfo(files[i]).Length;
            }
            //全体のデータサイズ
            req.ContentLength = contentLen + ((boundary.Length + crlf.Length + crlf.Length) * files.Count) + boundary.Length;

            //送信
            using (Stream reqStream = req.GetRequestStream())
            {
                for (int i = 0; i < files.Count; i++)
                {
                    //送信ファイル
                    using (FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        //ヘッダ
                        reqStream.Write(boundary, 0, boundary.Length);
                        reqStream.Write(crlf, 0, crlf.Length);
                        reqStream.Write(headers[i], 0, headers[i].Length);

                        //ファイル内容
                        byte[] buf = new byte[0x1000];
                        int readSize = 0;
                        while (true)
                        {
                            readSize = fs.Read(buf, 0, buf.Length);
                            if (readSize == 0) break;

                            reqStream.Write(buf, 0, readSize);
                        }

                        reqStream.Write(crlf, 0, crlf.Length);
                    }
                }
                reqStream.Write(boundary, 0, boundary.Length);
                WebResponse res = req.GetResponse();
            }
        }
        public byte[] PostMultipartFormData(string url, NameValueCollection query, NameValueCollection filepaths, Encoding enc)
        {
            var boundary = String.Format("_{0}_MULTIPART_MIXED_", System.Guid.NewGuid().ToString("N").ToUpper());
            this.Headers.Add(HttpRequestHeader.ContentType, "multipart/form-data; boundary=" + boundary);
            var nextBoundary = "--" + boundary;
            var data = new StringBuilder();
            foreach (string key in query.Keys)
            {
                data.AppendFormat("{0}\r\n", nextBoundary);
                data.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", key);
                data.Append("\r\n");
                data.AppendFormat("{0}\r\n", query[key]);
            }

            foreach (string key in filepaths.Keys)
            {
                data.AppendFormat("{0}\r\n", nextBoundary);
                data.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n", key, filepaths[key]);
                if (filepaths[key].Length == 0)
                {
                    data.Append("Content-Type: application/octet-stream\r\n");
                }
                else
                {
                    data.Append("Content-Type: application/vnd.ms-excel\r\n");
                }
                data.Append("\r\n");
                // テキストファイル読込
                using (var file = new StreamReader(filepaths[key], enc))
                {
                    data.Append(file.ReadToEnd());
                    file.Close();
                }
                data.Append("\r\n");
            }
            data.AppendFormat("{0}--\r\n", nextBoundary);
            return this.UploadData(url, enc.GetBytes(data.ToString()));
        }
        public byte[] PostMultipartFormData(string url, NameValueCollection query, HashSet<string> filepaths)
        {
            var boundary = String.Format("_{0}_MULTIPART_MIXED_", System.Guid.NewGuid().ToString("N").ToUpper());
            this.Headers.Add(HttpRequestHeader.ContentType, "multipart/form-data; boundary=" + boundary);
            var nextBoundary = "--" + boundary;
            var data = new StringBuilder();
            foreach (string key in query.Keys)
            {
                data.AppendFormat("{0}\r\n", nextBoundary);
                data.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", query[key]);
                data.Append("\r\n");
                data.Append("\r\n");
            }

            foreach (var filepath in filepaths)
            {
                data.AppendFormat("{0}\r\n", nextBoundary);
                data.AppendFormat("Content-Disposition: form-data; name=\"form1:fileupload1\"; filename=\"{0}\"\r\n", filepath);
                data.Append("Content-Type: application/vnd.ms-excel\r\n");
                data.Append("\r\n");
                // テキストファイル読込
                using (var file = new StreamReader(filepath))
                {
                    data.Append(file.ReadToEnd());
                    file.Close();
                }
                data.Append("\r\n");
            }

            data.AppendFormat("{0}\r\n", nextBoundary);
            data.Append("Content-Disposition: form-data; name=\"form1:register\"\r\n");
            data.Append("\r\n");
            data.Append("\r\n");
            data.AppendFormat("{0}\r\n", nextBoundary);
            data.AppendFormat("Content-Disposition: form-data; name=\"form1:isStarted\"\r\n");
            data.Append("\r\n");
            data.Append("\r\n");
            data.AppendFormat("{0}\r\n", nextBoundary);
            data.AppendFormat("Content-Disposition: form-data; name=\"form1\"\r\n");
            data.Append("\r\n");
            data.Append("form1\r\n");
            data.AppendFormat("{0}--\r\n", nextBoundary);

            return this.UploadData(url, Encoding.ASCII.GetBytes(data.ToString()));
        }
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.CookieContainer = this.CookieContainer;
                httpWebRequest.UserAgent = _UserAgent;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Timeout = System.Threading.Timeout.Infinite;
            }
            return webRequest;
        }
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            HttpWebResponse webres = null;

            try
            {
                //return base.GetWebResponse(request);
                //サーバーからの応答を受信するためのWebResponseを取得
                webres = (System.Net.HttpWebResponse)base.GetWebResponse(request);

                //応答したURIを表示する
                Debug.WriteLine(webres.ResponseUri);
                //応答ステータスコードを表示する
                Debug.WriteLine("{0}:{1}",
                    webres.StatusCode, webres.StatusDescription);
            }
            catch (System.Net.WebException ex)
            {
                //HTTPプロトコルエラーかどうか調べる
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    //HttpWebResponseを取得
                    System.Net.HttpWebResponse errres =
                        (System.Net.HttpWebResponse)ex.Response;
                    //応答したURIを表示する
                    Console.WriteLine(errres.ResponseUri);
                    //応答ステータスコードを表示する
                    Console.WriteLine("{0}:{1}",
                        errres.StatusCode, errres.StatusDescription);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
                //閉じる
                if (webres != null)
                {
                    webres.Close();
                }
            }
            return webres;
        }

//        public void Invoke(FormAction<Args> proc, Args args)
//        {
//            proc(args);
//        }

        #region FMWW.Helpers.Ajaxへ移動
        public static bool IsFin(string text)
        {
            return Core.Helpers.Ajax.IsFin(text);
        }

        protected static bool HasError(string text)
        {
            return Core.Helpers.Ajax.HasError(text);
        }

        protected static NameValueCollection CreateAjaxQuery()
        {
            return Core.Helpers.Ajax.CreateAjaxQuery();
        }

        private void Ajax(Uri address)
        {
            Core.Helpers.Ajax.Run(this, address);
        }
        #endregion

        //[Obsolete]
        //protected void RequestAjaxAsync(Uri address, object userState = null)
        //{
        //    this.UploadValuesAsync(address, Http.Method.Post, CreateAjaxQuery(), userState);
        //}
        //
        //[Obsolete]
        //public void AjaxAsync(Uri address, Action<object> completed, object userState = null)
        //{
        //    UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
        //    onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
        //        (o, args) =>
        //        {
        //            var html = Encoding.UTF8.GetString(args.Result);
        //            if (IsFin(html))
        //            {
        //                this.UploadValuesCompleted -= onUploadValuesCompleted;
        //                completed(args.UserState);
        //            }
        //            else
        //            {
        //                System.Threading.Thread.Sleep(1000);
        //                RequestAjaxAsync(address, userState);
        //            }
        //        });
        //    RequestAjaxAsync(address, userState);
        //    this.UploadValuesCompleted += onUploadValuesCompleted;
        //}
    }
}
