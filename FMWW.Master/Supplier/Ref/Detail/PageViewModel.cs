using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Master.Supplier.Ref.Detail
{
    class PageViewModel
    {
        // コード
        public string SupplierCode{get;set;}
        // 名称
        public string SupplierName{get;set;}
        // 略称
        public string SupplierNickname{get;set;}
        // カナ名
        public string SupplierKana{get;set;}
        // ローマ字名
        public string Roman{get;set;}
        //// 事業名
        //public string {get;set;}
        //// 事業カナ名
        //public string {get;set;}
        // 郵便番号
        public string PostalCode{get;set;}
        // 都道府県
        public string pref_nm{get;set;}
        // 住所1
        public string addr1{get;set;}
        // 住所2
        public string addr2{get;set;}
        // 電話番号
        public string tel{get;set;}
        // FAX
        public string fax{get;set;}
        // EMAIL
        public string email{get;set;}
        // 担当者１
        public string PersonInCharge1{get;set;}
        // 担当者２
        public string PersonInCharge2{get;set;}
        // 締日
        public string ClosedOn{get;set;}
        // 支払サイト
        public string usance{get;set;}
        //// 支払日
        //public string {get;set;}
        //// 決済サイト
        //public string {get;set;}
        //// 決済日
        //public string {get;set;}
        //// 支払区分
        //public string {get;set;}
        //// 仕入端数区分
        //public string {get;set;}
        //// 消費税区分
        //public string {get;set;}
        //// 仕入端数処理
        //public string {get;set;}
        //// 仕入端数金額
        //public string {get;set;}
        //// 消費税端数処理
        //public string {get;set;}
        //// 消費税端数金額
        //public string {get;set;}
        //// 自社担当者
        //public string {get;set;}
        //// 通貨区分
        //public string {get;set;}
        //// 仕入先ｸﾞｰﾙﾌﾟｺｰﾄﾞ
        //public string {get;set;}
        //// 税単位
        //public string {get;set;}
        //// 銀行コード
        //public string {get;set;}
        //// 管轄事業部１
        //public string {get;set;}
        //// 管轄事業部２
        //public string {get;set;}
        // 使用可能
        public string IsAvailable{get;set;}
        // 備考
        public string remark{get;set;}
    }
}
