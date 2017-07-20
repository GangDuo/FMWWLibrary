using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Master.BranchStore.Ref
{
    public class Context
    {
        // 得意先
        public Between<string> CustomerCode;
        // 納入先
        public Between<string> DestinationCode;
        // 納入先名
        public string DestinationName;
        // 営業担当者
        public string PersonCode;
        // 使用可能
        public bool Disable;

        public NameValueCollection Translate()
        {
            return new NameValueCollection()
            {
                {"form1:execute",    "execute"},
                {"form1:action",     "search"},
                {"form1:isAjaxMode", ""},
                {"customer1",        ""},
                {"customer2",        ""},
                {"dest_cd1",         ""},
                {"dest_cd2",         ""},
                {"dest_nm",          ""},
                {"person_cd",        ""},
                {"ser_disable_flg",  ""},
                {"form1",            "form1"},
            };
        }
    }
}
