using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Core
{
    public class ImportablePage : FMWW.Http.Page
    {
        protected static readonly string UrlE000Select = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("E000_IMPORT_GENERAL/E000_SELECT.jsp").AbsoluteUri;

        public string PathShiftJis { get; set; }

        public event Action<string> ImportCompleted;

        public ImportablePage() : base() { }
        public ImportablePage(FMWW.Http.Client client) : base(client) { }

        protected static NameValueCollection CreateFormData(bool isQuit = false)
        {
            var data = new NameValueCollection()
            {
                {"form1:isStarted", ""},
                {"form1",           "form1"},
            };
            if (isQuit)
            {
                data.Add("form1:quit", "");
            }
            else
            {
                data.Add("form1:register", "");
            }
            return data;
        }

        protected static NameValueCollection CreateEmptyFormData4File()
        {
            return new NameValueCollection()
            {
                {"form1:fileupload1", ""},
            };
        }

        //private static readonly string _CsvFileUTF8 = Path.GetTempFileName();
        protected NameValueCollection CreateFormData4File()
        {
            // Shift-JIS -> UTF8
            //using (var file = new StreamReader(PageContext.PathShiftJis, Encoding.GetEncoding("Shift_JIS")))
            //{
            //    var txt = file.ReadToEnd();
            //    Util.FileSystem.WriteText(_CsvFileUTF8, "UTF-8", txt);
            //}

            return new NameValueCollection()
            {
                {"form1:fileupload1", PathShiftJis},
            };
        }

        private static readonly Uri UrlImport = FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/Import");

        private static NameValueCollection CacheTime()
        {
            return new NameValueCollection() { { "cache", FMWW.Utility.UnixEpochTime.now().ToString() } };
        }

        private void ConfirmAsync()
        {
            _Client.UploadValuesAsync(UrlImport, Http.Method.Post, CacheTime());
        }

        public string Import()
        {
            var res = _Client.PostMultipartFormData(UrlE000Select, CreateFormData(), CreateFormData4File(), ShiftJIS);

            var polling = new Polling(_Client);
            polling.Wait(UrlImport, CacheTime);
            return polling.Parser.Message;
        }

        public void ImportAsync()
        {
            var res = _Client.PostMultipartFormData(UrlE000Select, CreateFormData(), CreateFormData4File(), ShiftJIS);
            Debug.WriteLine(Encoding.UTF8.GetString(res));
            // アップロード結果確認
            var polling = new Polling(_Client);
            polling.WaitAsync(UrlImport, CacheTime);
            Action<string> onCompleted = null;
            onCompleted = (message) =>
                {
                    polling.Completed -= onCompleted;
                    ImportCompletedIfPresence(message);
                };
            polling.Completed += onCompleted;
        }

        private void ImportCompletedIfPresence(string message)
        {
            if (null != ImportCompleted)
            {
                ImportCompleted(message);
            }
        }
    }
}
