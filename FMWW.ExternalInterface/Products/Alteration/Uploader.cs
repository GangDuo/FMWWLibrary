using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F = FMWW;

namespace FMWW.ExternalInterface.Products.Alteration
{
    public class Uploader
    {
        private F.ExternalInterface.Products.Alteration.Page _Page;

        public event Action<string/* result */> Uploaded;

        public void UploadAsync(string csvFileShiftJis)
        {
            _Page = new F.ExternalInterface.Products.Alteration.Page()
            {
                UserAccount = FMWW.Entity.Factory.UserAccount.Load(".user.json"),
                PageContext = new F.ExternalInterface.Products.Alteration.Context() { PathShiftJis = csvFileShiftJis }
            };
            _Page.Reached += OnReached;
            _Page.Registered += OnRegistered;
            _Page.ReachAsync();
        }

        private void OnReached()
        {
            _Page.Reached -= OnReached;

            _Page.Register();
        }

        private void OnRegistered(string result)
        {
            _Page.Registered -= OnRegistered;
            if (null != Uploaded)
            {
                Uploaded(result);
            }
        }

        public static Task<string> UploadTask(string csvFileShiftJIS)
        {
            var tcs = new TaskCompletionSource<string>();
            var uploader = new Uploader();
            uploader.Begin(csvFileShiftJIS, asyncResult =>
            {
                try { tcs.SetResult(uploader.End(asyncResult)); }
                catch (Exception exc) { tcs.SetException(exc); }
            }, null);
            return tcs.Task;
        }

        public virtual IAsyncResult Begin(string csvFileShiftJIS, AsyncCallback callback, object state)
        {
            var ar = new Context() { AsyncState = state, AsyncWaitHandle = null };
            //if (csvFilesShiftJIS.Count == 0)
            //{
            //    //AsyncCallback(IAsyncResult ar);
            //    callback(null);
            //    ar.CompletedSynchronously = true;
            //    ar.IsCompleted = true;
            //    return ar;
            //}
            //
            //var csvFileShiftJIS = csvFilesShiftJIS.Dequeue();
            //var productsUploader = new Advanced.FMWW.ExternalInterface.Products.Uploader();
            Action<string> OnProductsUploaded = null;
            OnProductsUploaded = (string result) =>
            {
                Uploaded -= OnProductsUploaded;
                ar.CompletedSynchronously = true;
                ar.IsCompleted = true;
                ar.UploadedResult = result;
                callback(ar);

                //var editor = new StringBuilder(result);
                //for (int i = 0; i < 3; i++)
                //{
                //    editor.AppendLine();
                //}
                //editor.AppendLine(@"--");
            };
            Uploaded += OnProductsUploaded;
            UploadAsync(csvFileShiftJIS);

            ar.CompletedSynchronously = false;
            ar.IsCompleted = false;
            return ar;
        }

        public virtual string End(IAsyncResult asyncResult)
        {
            if (asyncResult is Context)
            {
                return (asyncResult as Context).UploadedResult;
            }
            throw new Exception();
        }

        private class Context : IAsyncResult
        {
            public object AsyncState { get; set; }
            public System.Threading.WaitHandle AsyncWaitHandle { get; set; }
            public bool CompletedSynchronously { get; set; }
            public bool IsCompleted { get; set; }

            public string UploadedResult { get; set; }
        }
    }
}
