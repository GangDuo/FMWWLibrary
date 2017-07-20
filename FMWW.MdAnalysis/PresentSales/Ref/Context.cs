using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.MdAnalysis.PresentSales.Ref
{
    public class Context
    {
        public Between<DateTime> Date { get; set; }
        public string ShopCode { get; set; }
        public int RankWidth { get; set; }

        public Context()
        {
            RankWidth = 25;
        }

        public NameValueCollection Translate(bool isAjaxMode = false)
        {
            var today = DateTime.Today.ToString("yyyy年M月d日");
            var dateFrom = this.Date == null ? today : this.Date.From.ToString("yyyy年M月d日");
            var dateTo = this.Date == null ? today : this.Date.To.ToString("yyyy年M月d日");

            var nvc = new NameValueCollection()
            {
                {"date_001",	        dateFrom},// 売上期間 自
                {"date_002",	        dateTo},// 売上期間 至
                {"seasonCd",	        ""},    // シーズン
                {"lineCd",	            ""},    // ライン
                {"brandCd",	            ""},    // ブランド
                {"catgpCd",	            ""},    // アイテム
                {"area_cd",	            ""},    // 地区
                {"ps",	                "2"},   // P/S区分
                {"dest_class",	        ""},    // 店種
                {"dest_type",	        ""},    // 店格
                {"dest_cd:dest",	    ShopCode ?? String.Empty},    // 店舗 001
                {"dest_cd:destName",	""},
                {"dest_cd:cust",	    ""},
                {"dest_cd:area",	    ""},
                {"dest_cd:destClass",	""},
                {"dest_cd:destType",	""},
                {"dest_cd:destGroup",	""},
                {"supplier:sup",	    ""},    // 仕入先
                {"supplier:supName",	""},
                {"supplier:pref",	    ""},
                {"supplier:addr",	    ""},
                {"supplier:tel",	    ""},
                {"supplier:person",	    ""},
                {"supplier:bank",	    ""},
                {"sort_cd",	            "0"},   // ソート項目
                {"type_cd",	            "1"},   // 出力タイプ
                {"order_cd",	        "0"},   // ソート順
                {"start_rank",	        "1"},   // 順位の始まり
                {"rank_width",	        RankWidth.ToString()},  // 順位の終わり
                {"ViewUnit",	        "0"},   // 表示単位
                {"form1:execute",	    "execute"},
                {"form1:action",	    "export"},
                {"form1",	            "form1"},
                {"form1:isAjaxMode",	""},
            };
            if (isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
                nvc.Add("cons_001", "");    // 委託商品 ON
                nvc.Add("cons_002", "");    // 委託商品 OFF
            }
            return nvc;
        }
    }
}
