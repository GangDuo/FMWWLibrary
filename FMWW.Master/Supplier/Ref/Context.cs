using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Master.Supplier.Ref
{
    public class Context
    {
        public Between<string> SupplierCode = new Between<string>();
        public string SupplierName = "";

        public NameValueCollection Translate()
        {
            return new NameValueCollection()
            {
                {"form1:execute",    "execute"},
                {"form1:action",     "search"},
                {"form1:isAjaxMode", ""},
                {"sup_cd_from",      this.SupplierCode.From ?? ""},
                {"sup_cd_to",        this.SupplierCode.To ?? ""},
                {"ser_sup_nm",       ""},
                {"ser_closing_date", ""},
                {"ser_payment_site", ""},
                {"ser_payment_date", ""},
                {"ser_settle_site",  ""},
                {"ser_settle_date",  ""},
                {"ser_payment_cls",  ""},
                {"ser_person_cd",    ""},
                {"ser_disable_flg",  ""},
                {"form1",            "form1"},
            };
        }
    }
}
