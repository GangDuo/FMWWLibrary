using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.Core
{
    public class Polling : FMWW.Http.Page
    {
        public Script.Parser Parser { get; private set; }
        public event Action<string/* message */> Completed;

        public Polling(FMWW.Http.Client client) : base(client) { }

        public void Wait(Uri address, Func<NameValueCollection> factory)
        {
            string javascript = String.Empty;
            Parser = new Script.Parser();
            while (!FMWW.Http.Client.IsFin(javascript))
            {
                var res = _Client.UploadValues(address, Http.Method.Post, factory());
                javascript = Encoding.UTF8.GetString(res);
                Parser.Parse(javascript);
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void WaitAsync(Uri address, Func<NameValueCollection> factory)
        {
            UploadValuesCompletedEventHandler OnProgressChanged = null;
            OnProgressChanged = (o, args) =>
            {
                var javascript = Encoding.UTF8.GetString(args.Result);
                var parser = new Script.Parser();
                parser.Parse(javascript);

                if (FMWW.Http.Client.IsFin(javascript))
                {
                    _Client.UploadValuesCompleted -= OnProgressChanged;
                    CompletedIfPresence(parser.Message);
                    return;
                }
                ConfirmAsync(address, factory);
            };
            _Client.UploadValuesCompleted += OnProgressChanged;
            ConfirmAsync(address, factory);
        }

        private void ConfirmAsync(Uri address, Func<NameValueCollection> factory)
        {
            _Client.UploadValuesAsync(address, Http.Method.Post, factory());
        }

        private void CompletedIfPresence(string message)
        {
            if (null != Completed)
            {
                Completed(message);
            }
        }
    }
}
