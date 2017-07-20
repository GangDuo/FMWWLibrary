using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Serialization;

namespace FMWW.Inventory.Archive.Book.Ref
{
    [DataContract]
    public class Context
    {
        [DataMember(Name = "shopCode")]
        public string ShopCode { get; set; }

        [DataMember(Name = "lineCodes")]
        public string[] LineCodes { get; set; }

        [DataMember(Name = "itemCodes")]
        public string[] ItemCodes { get; set; }

        [DataMember(Name = "supplierCode")]
        public string SupplierCode { get; set; }

        /* http://yasuand.hatenablog.com/entry/2013/09/12/051655 */
        [DataMember(Name = "date")]
        private string date_str_prop
        {
            get { return date_str_field; }
            set
            {
                date_field = DateTime.ParseExact(value, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                date_str_field = value;
            }
        }
        private string date_str_field;
        public DateTime Date
        {
            get { return date_field; }
            set
            {
                date_str_field = value.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                date_field = value;
            }
        }
        private DateTime date_field { set; get; }

        [DataMember(Name = "mode")]
        public RefMode Mode { get; set; }

        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        // 品番
        [DataMember(Name = "styleCode")]
        public string StyleCode { get; set; }

        [DataMember(Name = "brandCodes")]
        public string[] BrandCodes { get; set; }

        // Delegate
        //public BuildQuery BuildQuery;
        public enum RefMode
        {
            JUST_NOW = 0,  // 現在庫
            THAT_DAY = 1   // 時点在庫
        }

        public NameValueCollection Translate()
        {
            var shopCode = this.ShopCode ?? "";
            var lineCodes = this.LineCodes ?? new string[] { "" };
            var itemCodes = (null == this.ItemCodes) ? "" : String.Join("\t", this.ItemCodes);
            var brandCodes = (null == this.BrandCodes) ? "" : String.Join("\t", this.BrandCodes);
            var supplierCode = this.SupplierCode ?? "";
            var nvc = new NameValueCollection() {
                    {"form1:search",           "検索"},
                    {"location_cd1:SELECT",    shopCode},
                    {"location_cd1:FEELD",     shopCode},
                    {"location_cd1:PROP_RATE", "100"},
                    {"location_cd1:SALE_RATE", "100"},
                    {"location_cd2:SELECT",    shopCode},
                    {"location_cd2:FEELD",     shopCode},
                    {"location_cd2:PROP_RATE", "100"},
                    {"location_cd2:SALE_RATE", "100"},
                    {"season_cd",              ""},
                    {"line_cd",                String.Join("\t", lineCodes)},
                    {"brand_cd",               brandCodes},
                    {"item_cd",                itemCodes},
                    {"sup_cd:FEELD",           supplierCode},
                    {"form1:style_attr1",      ""},
                    {"form1:Product_cd1",      ""},
                    {"form1:style_attr2",      ""},
                    {"form1:Style_cd1",        ""},
                    {"form1:style_attr3",      ""},
                    {"Color_cd:FEELD",         ""},
                    {"form1:sku_attr1",        ""},
                    {"Size_cd:FEELD",          ""},
                    {"form1:sku_attr2",        ""},
                    {"date_001:DATE",          this.Date.ToString("yyyy年M月d日")},
                    {"form1:sku_attr3",        ""},
                    {"form1:mode_001",         "1"},
                    {"form1:sign_001",         "on"},
                    {"form1:sign_002",         "on"},
                    {"form1:display_001",      "0"},
                    {"form1",                  "form1"}
                };
            if (supplierCode.Length > 0)
            {
                nvc.Add("sup_cd:SELECT", supplierCode);
            }
            if (Context.RefMode.JUST_NOW == this.Mode)
            {
                nvc.Remove("date_001:DATE");
                nvc["form1:mode_001"] = "0";
                nvc["form1:Product_cd1"] = this.Sku ?? "";
            }
            return nvc;
        }
    }

    //delegate string BuildQuery();
}
