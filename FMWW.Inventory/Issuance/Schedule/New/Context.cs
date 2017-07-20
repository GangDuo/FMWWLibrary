using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FMWW.Inventory.Issuance.Schedule.New
{
    public class Context
    {
        public NameValueCollection Translate()
        {
            return new NameValueCollection() {
                    {"form1:execute",      "execute"},
                    {"form1:action",       "execute"},
                    {"stocktaking_date",   "2014年6月10日"},
                    {"location:dest",      ""},
                    {"location:destName",  ""},
                    {"location:cust",      ""},
                    {"location:area",      ""},
                    {"location:destClass", ""},
                    {"location:destType",  ""},
                    {"location:destGroup", ""},
                    {"form1:check01",      ""},
                    {"adjust",             "0"},
                    {"stockTableUpdate",   "1"},
                    {"form1",              "form1"},
                    {"form1:isAjaxMode",   "1"},
                    {"form1",              "form1"},
                    {"form1:execute",      "execute"},
                    {"cache", FMWW.Utility.UnixEpochTime.now().ToString()},
                };
        }
    }
}
