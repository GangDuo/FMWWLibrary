using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

// 外部ｲﾝﾀｰﾌｪｲｽ -> 商品ﾏｽﾀﾒﾝﾃﾅﾝｽ -> 照会
namespace FMWW.ExternalInterface.Products.Ref
{
    public class Context
    {
        public string FormAction { get; set; }
        public Between<DateTime> CreationDate { get; set; }
        public Between<DateTime> EditingDate { get; set; }
        public Between<string> ModelNo { get; private set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string Barcode2 { get; set; }
        public string SupplierCode { get; set; }
        public string SeasonCode { get; set; }
        public string BrandCode { get; set; }
        public string ItemCode { get; set; }
        public string LineCode { get; set; }
        public string Intention { get; set; }
        public bool On { get; set; }
        public bool Off { get; set; }

        public Context()
        {
            this.FormAction = "export";
            this.CreationDate = null;
            this.EditingDate = null;
            this.ModelNo = new Between<string>() { From = "", To = "" };
            this.ProductName = "";
            this.Barcode = "";
            this.Barcode2 = "";
            this.SupplierCode = "";
            this.SeasonCode = "";
            this.BrandCode = "";
            this.ItemCode = "";
            this.LineCode = "";
            this.Intention = "";
            this.On = false;
            this.Off = false;
        }

        public NameValueCollection Translate(bool isAjaxMode = false)
        {
            var createDateFrom = this.CreationDate == null ? "" : this.CreationDate.From.ToString("yyyy年M月d日");
            var createDateTo = this.CreationDate == null ? "" : this.CreationDate.To.ToString("yyyy年M月d日");
            var modifyDateFrom = this.EditingDate == null ? "" : this.EditingDate.From.ToString("yyyy年M月d日");
            var modifyDateTo = this.EditingDate == null ? "" : this.EditingDate.To.ToString("yyyy年M月d日");

            var nvc = new NameValueCollection()
            {
                {"form1:execute", "execute"},
                {"form1:action", this.FormAction},
                {"style_cd", ""},
                {"style_cd2", ""},
                {"style_nm", ""},
                {"barcode", this.Barcode},
                {"barcode2", ""},
                {"supplier_list", ""},
                {"season_list", SeasonCode},
                {"brand_list", ""},
                {"item_list", this.ItemCode},
                {"line_list", LineCode},
                {"intention", ""},
                {"form1", "form1"},
                {"form1:isAjaxMode", ""},
                {"createdate_from", createDateFrom},
                {"createdate_to", createDateTo},
                {"modifydate_from", modifyDateFrom},
                {"modifydate_to", modifyDateTo},
            };
            if (isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
                nvc.Add("form1:disable_on", "");
                nvc.Add("form1:disable_off", "");
            }
            return nvc;
        }
    }
}
