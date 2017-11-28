using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.PurchasingExpenses.Ref
{
    public class Context
    {
        //Nullable<DateTime>
        public Between<DateTime?> MovementDate { get; private set; }
        public Between<string> Location { get; private set; }

        public Context()
        {
            MovementDate = new Between<DateTime?>();
            Location = new Between<string>() { From = String.Empty, To = String.Empty };
        }

        public NameValueCollection Translate()
        {
            string from = String.Empty;
            string to = String.Empty;
            if (null != MovementDate)
            {
                const string f = "yyyy年M月d日";
                if (this.MovementDate.From.HasValue)
                {
                    from = this.MovementDate.From.Value.ToString(f);
                }
                if (this.MovementDate.To.HasValue)
                {
                    to = this.MovementDate.To.Value.ToString(f);
                }
            }
            return new NameValueCollection()
            {
                {"form1:execute",      "execute"},
                {"form1:action",       "execute"},
                {"form1:isAjaxMode",   ""},
                {"date_from",          from},
                {"date_to",            to},
                {"out_loc:dest",       Location.From}, // 出荷元
                {"out_loc:destName",   ""},	
                {"out_loc:cust",       ""},	
                {"out_loc:area",       ""},	
                {"out_loc:destClass",  ""},	
                {"out_loc:destType",   ""},	
                {"out_loc:destGroup",  ""},	
                {"in_loc:dest",        Location.To }, // 出荷先
                {"in_loc:destName",    ""},	
                {"in_loc:cust",        ""},	
                {"in_loc:area",        ""},	
                {"in_loc:destClass",   ""},	
                {"in_loc:destType",    ""},	
                {"in_loc:destGroup",   ""},	
                {"sup_cd",             ""}, // 仕入先
                {"catg_gp_cd",         ""},	
                {"style_nm",           ""},	
                {"barcode",            ""},	
                {"prod_cd",            ""},	
                {"form1",              "form1"},
            };
        }
    }
}
