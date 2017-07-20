using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.ExternalInterface.Inventory.New
{
    // 外部ｲﾝﾀｰﾌｪｲｽ -> 棚卸ｲﾝﾎﾟｰﾄ -> 入力
    public class Page : FMWW.Http.AbstractUploader
    {
        public Page() : base() { }
        public Page(FMWW.Http.Client client) : base(client) { }

        public override void Register()
        {
            Execute(null);
        }

        protected override FMWW.Core.MainMenu SelectMainMenu()
        {
            return MainMenuFactory.CreateInstance();
        }
    }
}
