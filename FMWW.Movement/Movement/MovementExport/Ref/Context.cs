using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Movement.Movement.MovementExport.Ref
{
    public class Context
    {
        public Between<Nullable<DateTime>> MovementDate { get; set; }
        // 出荷元
        public List<string> Load { get; private set; }
        // 出荷先
        public List<string> Unload { get; private set; }

        public Context()
        {
            this.Load = new List<string>();
            this.Unload = new List<string>();
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
            const string separator = "\t";
            string load = String.Join(separator, this.Load.Distinct());
            string unload = String.Join(separator, this.Unload.Distinct());

            return new NameValueCollection()
            {
                {"form1:execute",      "execute"},
                {"form1:action",       "execute"},
                {"form1:isAjaxMode",   ""},
                {"moving_date_from",   from},
                {"moving_date_to",     to},
                {"destFrom:dest",      load},
                {"destFrom:destName",  ""},
                {"destFrom:cust",      ""},
                {"destFrom:area",      ""},
                {"destFrom:destClass", ""},
                {"destFrom:destType",  ""},
                {"destFrom:destGroup", ""},
                {"destTo:dest",        unload},
                {"destTo:destName",    ""},
                {"destTo:cust",        ""},
                {"destTo:area",        ""},
                {"destTo:destClass",   ""},
                {"destTo:destType",    ""},
                {"destTo:destGroup",   ""},
                {"form1",              "form1"},
            };
        }
    }
}
