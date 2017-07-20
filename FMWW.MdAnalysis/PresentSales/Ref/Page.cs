using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FMWW.MdAnalysis.PresentSales.Ref
{
    public class Page : FMWW.Http.Page
    {
        public event Action<string/* html */> Reached;

        public Context PageContext { get; set; }

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        // MD分析 -> 店舗売上集計 -> 照会
        public string Reach()
        {
            var binary = _Client.UploadValues(FMWW.Core.MainMenu.Url, "POST", MainMenuFactory.CreateInstance().Translate());
            var html = Encoding.UTF8.GetString(binary);
            return html;
        }

        public void ReachAsync()
        {
            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     var html = Encoding.UTF8.GetString(args.Result);
                     if (null != Reached)
                     {
                         Reached(html);
                     }
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, "POST", MainMenuFactory.CreateInstance().Translate());
        }

        public override string Csv()
        {
            SignIn();
            Reach();

            var address = FMWW.Core.Helpers.UrlBuilder.BuildContentsUrl("NA250_SALES_PRESENT_TTL/NA250_SELECT.jsp");
            //_Client.UploadValues(address, c.Translate(true));
            //_Client.AjaxForSearch(Core.Client.BuildUrl(address.AbsolutePath.Replace("faces", "facesAjax")));            
            var javascript = Encoding.UTF8.GetString(_Client.UploadValues(address, PageContext.Translate(true)));
            if (!FMWW.Http.Client.IsFin(javascript))
            {
                var polling = new FMWW.Core.Polling(_Client);
                polling.Wait(FMWW.Core.Helpers.UrlBuilder.Build(address.AbsolutePath.Replace("faces", "facesAjax")),
                    () =>
                    {
                        return new System.Collections.Specialized.NameValueCollection()
                        {
                            {"form1:isAjaxMode",	"1"},
                            {"form1",	            "form1"},
                            {"form1:execute",	    "execute"},
                            {"cache",	            FMWW.Utility.UnixEpochTime.now().ToString()},
                        };
                    });
            }

            var buf = _Client.UploadValues(address, PageContext.Translate());
            return ShiftJIS.GetString(buf);    
        }
    }
}
