using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.MdAnalysis.SalesRanking.Ref
{
    static class FormActionExt
    {
        public static string DisplayName(this Context.FormAction formAction)
        {
            string[] names = { "search", "export" };
            return names[(int)formAction];
        }
    }

    public class Context
    {
        public enum FormAction { Search, Export };

        public Between<Nullable<DateTime>> Date { get; set; }
        public string StyleCode { get; set; }

        public Context()
        {
            StyleCode = String.Empty;
        }

        public NameValueCollection Translate(FormAction formAction, bool isAjaxMode = false)
        {
            const string f = "yyyy年M月d日";
            string from = DateTime.Today.AddDays(-1.0).ToString(f);
            string to = DateTime.Today.AddDays(-1.0).ToString(f);
            if (null != Date)
            {
                if (this.Date.From.HasValue)
                {
                    from = this.Date.From.Value.ToString(f);
                }

                if (this.Date.To.HasValue)
                {
                    to = this.Date.To.Value.ToString(f);
                }
            }

            var nvc = new NameValueCollection()
            {
                {"form1:execute",         "execute"},
                {"form1:action",          formAction.DisplayName()},
                {"form1:isAjaxMode",      ""},
                {"salesDateFrom",         from},
                {"salesDateTo",           to},
                {"styleCd",               StyleCode},
                {"seasonCd",              ""},
                {"brandCd",               ""},
                {"catgpCd",               ""},//205
                {"supCd",                 ""},
                {"destCd:dest",           ""},
                {"destCd:destName",       ""},
                {"destCd:cust",           ""},
                {"destCd:area",           ""},
                {"destCd:destClass",      ""},
                {"destCd:destType",       ""},
                {"destCd:destGroup",      ""},
                {"lineCd",                ""},
                {"dest_type",             ""},
                {"dest_class",            ""},
                {"dest_area",             ""},
                {"form1:psClass",         "2"},
                {"form1:ranking",         "0"},
                {"form1:placeUnit",       "1"},
                {"form1:sort",            "1"},
                {"form1",                 "form1"},
            };
            if(isAjaxMode)
            {
                nvc["form1:isAjaxMode"] = "1";

                nvc.Add("form1:cons_001", "");
                nvc.Add("form1:cons_002", "");
                nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());
            }
            return nvc;

        }
    }
}
