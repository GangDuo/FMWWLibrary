using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FMWW.Core;

namespace FMWW.Ordering.Shop.Ref
{
    public class Context
    {
        public FMWW.Core.CenterType? CenterType { get; set; }
        public FMWW.Component.Between<System.Nullable<DateTime>> OrderDate = new FMWW.Component.Between<DateTime?>();

        public NameValueCollection Translate(bool isAjaxMode = false)
        {
            var yesterday = DateTime.Today.AddDays(-1.0);
            var nvc = new NameValueCollection()
            {
                {"form1:execute",	    "execute"},
                {"form1:action",	    "export"},
                {"_order_start_date",	OrderDate.From.HasValue ? OrderDate.From.Value.ToString("yyyy年M月d日") : yesterday.ToString("yyyy年M月d日")},
                {"_order_end_date",	    OrderDate.To.HasValue ? OrderDate.To.Value.ToString("yyyy年M月d日") : yesterday.ToString("yyyy年M月d日")},
                {"_dest",	            ""},
                {"_supplier",	        ""},
                {"_style",	            ""},
                {"_sku",	            ""},
                {"_catg",	            ""},
                {"_stock",	            ""},
                {"customerOrderClass",	""},
                {"skuAttr1",	        "0"},   // 廃番品区分
                {"intention",	        CenterType.Code()},    // 商品区分
                {"material",	        ""},
                {"person",	            ""},
                {"form1",	            "form1"},
                {"form1:isAjaxMode",	""},
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
