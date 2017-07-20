using FMWW.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.ScheduledArrival.Distribute.Ref
{
    public class Context
    {
        // 投入表番号
        public Between<string> Code = new Between<string>();
        // 作成更新日付
        public Between<DateTime> CreationDate = new Between<DateTime>();
        // 掛計上日付
        public Between<DateTime> PaymentDate = new Between<DateTime>();
        // 物流入荷予定日付
        public Between<DateTime> WhScheDate = new Between<DateTime>();
        // 店舗入荷予定日付
        public Between<DateTime> ShopScheDate = new Between<DateTime>();
        public string SupplierCode = "";
        public string LineCode = "";
        public HashSet<string> ItemCodes = new HashSet<string>();
        public string SeasonCode = "";
        public string BrandCode = "";
        public string ModelNo = "";
//        public string ModelNo = "";
        public string Status = "";
//        public Context.Route Route;

        public struct Route
        {
            public string Start;
            public List<string> Goals;
        }
    }
}
