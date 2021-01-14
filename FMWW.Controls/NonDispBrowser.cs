using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FMWW.Controls
{
    public class NonDispBrowser : WebBrowser
    {
        //public Queue<string> DistributeCodes { get; private set; }
        private Queue<WebBrowserDocumentCompletedEventHandler> _documentCompletedEventHandler = new Queue<WebBrowserDocumentCompletedEventHandler>();
        private bool done;

        // タイムアウト時間（10秒）
        TimeSpan timeout = new TimeSpan(0, 0, 10);

        //protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        //{
        //    // ページにフレームが含まれる場合にはフレームごとに
        //    // このメソッドが実行されるため実際のURLを確認する
        //    if (e.Url == this.Url)
        //    {
        //        done = true;
        //    }
        //}

        protected override void OnNewWindow(System.ComponentModel.CancelEventArgs e)
        {
            // ポップアップ・ウィンドウをキャンセル
            e.Cancel = true;
        }

        public void AddDocumentCompletedEventHandler(WebBrowserDocumentCompletedEventHandler handler)
        {
            _documentCompletedEventHandler.Enqueue(handler);
            //this.DocumentCompleted += handler;
        }

        //public void RemoveDocumentCompletedEventHandler()
        //{
        //    this.DocumentCompleted -= _documentCompletedEventHandler.Dequeue();
        //}

        public NonDispBrowser()
        {
            //this.DistributeCodes = new Queue<string>();
            // スクリプト・エラーを表示しない
            this.ScriptErrorsSuppressed = true;

            this.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(args.Url.ToString());
                        var func = _documentCompletedEventHandler.Dequeue();
                        func(sender, args);
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        System.Diagnostics.Debug.Write(ex.Message);
                        System.Diagnostics.Debug.WriteLine(args.Url.ToString());
                    }
                });
        }

        public bool NavigateAndWait(string url)
        {

            base.Navigate(url); // ページの移動

            done = false;
            DateTime start = DateTime.Now;

            while (done == false)
            {
                if (DateTime.Now - start > timeout)
                {
                    // タイムアウト
                    return false;
                }
                Application.DoEvents();
            }
            return true;
        }
    }
}
