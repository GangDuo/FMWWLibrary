using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.ScheduledArrival.DistributeExport.Ref
{
    public class Context
    {
        //
        // 概要:
        //     文字列のアップロードとダウンロードに使用する System.Text.Encoding を取得または設定します。
        //
        // 戻り値:
        //     投入表番号。
        //
        public string Code { get; set; }
        // 作成更新日付
        public Between<Nullable<DateTime>> CreationDate { get; set; }
        // 店舗入荷予定日付
        public Between<Nullable<DateTime>> ShopScheDate { get; set; }
        public string StoreCode { get; set; }
        public string SupplierCode { get; set; }
        public string ModelNo { get; set; }
        public string Sku { get; set; }
        public HashSet<string> ItemCodes { get; private set; }
        public string Division { get; set; }

        public Context()
        {
            this.ItemCodes = new HashSet<string>();
            this.Code = "";
            this.StoreCode = "";
            this.SupplierCode = "";
            this.ModelNo = "";
            this.Sku = "";
            this.Division = "";
//            this.CreationDate = new Between<DateTime>();
        }

        public NameValueCollection Translate(bool isAjaxMode = false)
        {
            var creationDateFrom = "";
            var creationDateTo = "";
            var shopScheDateFrom = "";
            var shopScheDateTo = "";

            if ((null != this.CreationDate) && (this.CreationDate.From.HasValue))
            {
                creationDateFrom = this.CreationDate.From.Value.ToString("yyyy年M月d日");
            }
            if ((null != this.CreationDate) && (this.CreationDate.To.HasValue))
            {
                creationDateTo = this.CreationDate.To.Value.ToString("yyyy年M月d日");
            }
            if ((null != this.ShopScheDate) && (this.ShopScheDate.From.HasValue))
            {
                shopScheDateFrom = this.ShopScheDate.From.Value.ToString("yyyy年M月d日");
            }
            if ((null != this.ShopScheDate) && (this.ShopScheDate.To.HasValue))
            {
                shopScheDateTo = this.ShopScheDate.To.Value.ToString("yyyy年M月d日");
            }
            var nvc = new NameValueCollection()
            {
                {"form1:execute",           "execute"},
                {"form1:action",            "export"},
                {"_distribute_no",          this.Code},
                {"_order_start_date",       creationDateFrom},
                {"_order_end_date",         creationDateTo},
                {"shop_arrival_start_date", shopScheDateFrom},
                {"shop_arrival_end_date",   shopScheDateTo},
                {"_dest",                   this.StoreCode},
                {"_supplier",               this.SupplierCode},
                {"_style",                  this.ModelNo},
                {"_sku",                    this.Sku},
                {"_catg",                   String.Join("\t", this.ItemCodes)},
                {"_stock",                  this.Division},
                {"form1",                   "form1"},
                {"form1:isAjaxMode",        ""},
            };
            if (isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
            }
            return nvc;
        }
    }
}
