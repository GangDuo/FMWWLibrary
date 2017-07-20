using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Mobility.Ordering
{
    public class Context
    {
        public List<string> LineCodes;
        public List<string> ItemCodes;
        public List<string> SupplierCodes;
        public string ProductName = "";
        public string ModelNo = "";
        public string Barcode = "";
        public string SeasonCode = "";
        public string BrandCode = "";
        public Between<int> Stock = new Between<int>();

        public NameValueCollection Translate(FormAction action = FormAction.Search)
        {
            var nvc = new NameValueCollection()
                {
                    { "form1:execute",    "execute"},
                    { "form1:action",     ""},
                    { "form1:isAjaxMode", ""},
                    { "form1:clickRow",   ""},
                    { "line",             ""},
                    { "catg_gp",          ""},
                    { "sup_cd",           ""},
                    { "style_nm",         ""},
                    { "season_cd",        ""},
                    { "style_cd",         ""},
                    { "brand_cd",         ""},
                    { "barcode_cd",       this.Barcode},
                    { "sku_class",        ""},
                    { "sstockCountFrom",  ""},
                    { "sstockCountTo",    ""},
                    { "form1",            "form1"},
                };
            switch (action)
            {
                case FormAction.Search:
                    nvc["form1:action"] = "search";
                    nvc["form1:isAjaxMode"] = "1";

                    nvc.Add("form1:sort_flg", "");
                    nvc.Add("form1:display_flg", "");
                    nvc.Add("cache", FMWW.Utility.UnixEpochTime.now().ToString());

                    break;
                case FormAction.Preview:
                    nvc["form1:action"] = "preview";
                    break;
                default:
                    throw new Exception();
            }
            return nvc;
        }

        public enum FormAction
        {
            Search,
            Preview
        }

        //public static class FormActionExt
        //{
        //    // Gender に対する拡張メソッドの定義
        //    public static string DisplayName(this FormAction action)
        //    {
        //        string[] names = { "search", "preview" };
        //        return names[(int)action];
        //    }
        //}
    }
}
