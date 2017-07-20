using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F = FMWW;

namespace FMWW.ForShop.Work.Journals.Ref
{
    public class Downloader
    {
        public event Action<string/* result */> Downloaded;

        private F.ForShop.Work.Journals.Ref.Page page;

        private void OnReached(string html)
        {
            page.Reached -= OnReached;
            page.ExcelDownloadCompleted += OnExcelDownloadCompleted;
            page.ExcelAsync();
        }

        private void OnExcelDownloadCompleted(byte[] binary)
        {
            page.ExcelDownloadCompleted -= OnExcelDownloadCompleted;

            //var filename = Text.RandomString.Generate() + ".xlsx";
            //if (client.ResponseHeaders.AllKeys.Contains("Content-Disposition"))
            //{
            //    var m = Regex.Match(client.ResponseHeaders["Content-Disposition"], @"filename=([^\.]+\.xlsx)", RegexOptions.IgnoreCase);
            //    if (m.Success && (m.Groups.Count >= 2))
            //    {
            //        filename = m.Groups[1].Value;
            //    }
            //}

            var saveTo = Path.Combine(Path.GetTempPath(), Text.RandomString.Generate() + ".xlsx");
            Util.FileSystem.WriteBinary(saveTo, binary);
            if (null != Downloaded)
            {
                Downloaded(saveTo);
            }
        }

        public void DownloadAsync(F.ForShop.Work.Journals.Ref.Context context)
        {
            page = new F.ForShop.Work.Journals.Ref.Page()
            {
                UserAccount = Entity.Factory.UserAccount.Load(".user.json"),
                PageContext = context
            };
            //page.Reached += OnReached;
            //page.ReachAsync();
            page.ExcelDownloadCompleted += OnExcelDownloadCompleted;
            page.ExcelAsync();
        }

        public static Task<string> DownloadTask(F.ForShop.Work.Journals.Ref.Context context)
        {
            var tcs = new TaskCompletionSource<string>();
            var downloader = new Downloader();
            downloader.Begin(context, asyncResult =>
            {
                try { tcs.SetResult(downloader.End(asyncResult)); }
                catch (Exception exc) { tcs.SetException(exc); }
            }, null);
            return tcs.Task;
        }

        public virtual IAsyncResult Begin(F.ForShop.Work.Journals.Ref.Context context, AsyncCallback callback, object state)
        {
            var ar = new Context() { AsyncState = state, AsyncWaitHandle = null };
            Action<string> onDownloaded = null;
            onDownloaded = (string result) =>
            {
                Downloaded -= onDownloaded;
                ar.CompletedSynchronously = true;
                ar.IsCompleted = true;
                ar.Result = result;
                callback(ar);
            };
            Downloaded += onDownloaded;
            DownloadAsync(context);

            ar.CompletedSynchronously = false;
            ar.IsCompleted = false;
            return ar;
        }

        public virtual string End(IAsyncResult asyncResult)
        {
            if (asyncResult is Context)
            {
                return (asyncResult as Context).Result;
            }
            throw new Exception();
        }

        private class Context : IAsyncResult
        {
            public object AsyncState { get; set; }
            public System.Threading.WaitHandle AsyncWaitHandle { get; set; }
            public bool CompletedSynchronously { get; set; }
            public bool IsCompleted { get; set; }

            public string Result { get; set; }
        }
    }
}
