using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.ExternalInterface.Products.Alteration
{
    // 外部ｲﾝﾀｰﾌｪｰｽ -> 商品一括修正ｲﾝﾎﾟｰﾄ -> 実行
    public class Page : FMWW.Core.ImportablePage
    {
        public Context PageContext { get; set; }
        //public string Result { get; private set; }

        public string HtmlText { get; private set; }
        public event Action Reached;
        public event Action<string> Registered;

        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public void ReachAsync()
        {
            SignIn();

            UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
            onUploadValuesCompleted = new UploadValuesCompletedEventHandler(
                 (o, args) =>
                 {
                     _Client.UploadValuesCompleted -= onUploadValuesCompleted;
                     HtmlText = ShiftJIS.GetString(args.Result);
                     if (null != Reached)
                     {
                         Reached();
                     }
                 });
            _Client.UploadValuesCompleted += onUploadValuesCompleted;
            _Client.UploadValuesAsync(FMWW.Core.MainMenu.Url, "POST", MainMenuFactory.CreateInstance().Translate());
        }

        public override void Quit()
        {
            _Client.PostMultipartFormData(UrlE000Select, CreateFormData(true), CreateEmptyFormData4File(), Encoding.UTF8);
        }

        public override void Register()
        {
            // 変更履歴へ登録
            RevisionHistory.Persistence.Register(PageContext.PathShiftJis);
            // 一括修正csvアップロード
            PathShiftJis = PageContext.PathShiftJis;
            ImportCompleted += (result) =>
            {
                if (null != Registered)
                {
                    Registered(result);
                }
            };
            ImportAsync();
        }
    }
}
