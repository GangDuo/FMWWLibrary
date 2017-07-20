using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//店舗管理 -> 店舗顧客 -> ポイント入力
namespace FMWW.ForShop.Customers.AdditionalPoint.New
{
    public class Context
    {
        //
        // 会員番号
        public string MemberId;
        //
        // レシート番号
        public string ReceiptNo;
        //
        // ポイント発行日
        public DateTime PointAdditionDate;
        //
        // 発行店舗
        public string ShopCode;
        //
        // 入力担当者
        public string PersonCode;
        //
        // 累積加算
        public string AddTotal;
        //
        // 発行ポイント
        public int AdditionalPoint;
        //
        // 事由
        public string Reason;
        //
        // 備考
        public string Remark;
    }
}
