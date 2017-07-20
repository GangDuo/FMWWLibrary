using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.ForShop.Work.Journals.Ref
{
    public class Context
    {
        // 出力形式はcsv
        public bool AsCsv { get; set; }
        // 売上期間
        public Between<System.Nullable<DateTime>> PeriodOfSales = new Between<System.Nullable<DateTime>>();
        //// 品番
        //public string ModelNo { get; set; }
        //// SKU
        //public string Sku { get; set; }
        //// シーズン
        //public HashSet<string> SeasonCode;
        //// ブランド
        //public HashSet<string> BrandCode;
        //// ライン
        //public HashSet<string> LineCode;
        //// アイテム
        //public HashSet<string> ItemCodes;
        //
        //// 仕入先
        //public string SupplierCode { get; set; }
        //// P/S区分
        //public ProperSale Ps;
        // 店舗
        public string StoreCode { get; set; }



        public NameValueCollection Translate(bool isAjaxMode = false)
        {
            var nvc = new NameValueCollection()
            {
                {"form1:execute",    "execute"},
                {"form1:action",     "export"},
                {"sales_date_from",  PeriodOfSales.From.HasValue ? PeriodOfSales.From.Value.ToString("yyyy年M月d日") : String.Empty},
                {"sales_date_to",    PeriodOfSales.To.HasValue ? PeriodOfSales.To.Value.ToString("yyyy年M月d日") : String.Empty},
                {"slip_no",          String.Empty},
                {"local_slip_no",    String.Empty},
                {"dest:dest",        String.Empty},
                {"dest:destName",    String.Empty},
                {"dest:cust",        String.Empty},
                {"dest:area",        String.Empty},
                {"dest:destClass",   String.Empty},
                {"dest:destType",    String.Empty},
                {"dest:destGroup",   String.Empty},
                {"person1",          String.Empty},
                {"person2",          String.Empty},
                {"trade_class",      String.Empty},
                {"shopper_cd",       String.Empty},
                {"shopper_person",   String.Empty},
                {"remark",           String.Empty},
                {"form1:ps",         "2"},
                {"prodCode",         String.Empty},
                {"styleCode",        String.Empty},
                {"size",             String.Empty},
                {"color",            String.Empty},
                {"brand",            String.Empty},
                {"catg",             String.Empty},
                {"sup",              String.Empty},
                {"season",           String.Empty},
                {"form1:sum",        "0"},// 集計方法
                {"form1:type",       "1"},// 出力単位
                {"form1",            "form1"},
                {"form1:isAjaxMode", String.Empty},
            };

            if (isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";
                nvc.Add("form1:dispOnlyShopper", String.Empty);
                nvc.Add("form1:dispOnlyInner", String.Empty);
                nvc.Add("form1:dispOnlyCredit", String.Empty);
                nvc.Add("form1:sale001", String.Empty);
                nvc.Add("form1:sale002", String.Empty);
                nvc.Add("form1:summaryType", String.Empty);// これらの商品を含む伝票は、その明細をすべて集計する。
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
            }
            return nvc;
        }
    }

    public enum ProperSale
    {
        Proper,
        Sale,
        ProperAndSale,
    }
}
