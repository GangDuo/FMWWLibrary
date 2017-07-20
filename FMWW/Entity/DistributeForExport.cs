using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FMWW.Entity
{
    public class DistributeForExport
    {
        public UInt64 Code { get; set; }                  // 投入表番号
        public UInt32 BranchId { get; set; }              // 伝票行番号
        public DateTime WhScheDate { get; set; }          // 
        public DateTime ShopArrivalDate { get; set; }     // 店舗納品予定日
        public         DateTime d;     // 掛計上日
        public         String s;               // 出荷元
        public String StoreCode { get; set; }             // 店舗コード
        public String SupplierCode { get; set; }          // 仕入先
        public Int32 ItemCode { get; set; }               // アイテム
        public String ModelNumber { get; set; }           // 品番
        public String ProductName { get; set; }           // 品名
        public String ColorCD { get; set; }               // 色コード
        public String SizeCD { get; set; }                // サイズコード
        public String Sku { get; set; }                   // 
        public Decimal Quantity { get; set; }             // 投入数量
        public Decimal SuggestedRetailPrice { get; set; } // 上代単価
        public Decimal Cost { get; set; }                 // 下代単価
                // 作成日付
                // 修正日付
                // 削除日付
                // 更新フラグ
                // 総数量
                // 仕入単価
                // 商品区分
        
        public static DataTable Convert(string csv)
        {
            var table = FMWW.Entity.Distribute.CreateTemporaryTable();
            var colIndexes = new Dictionary<string, int>()
                        {
                            { "distributeNo",          0 },     
                            { "branchid",              1 },     
                            { "wh_sche_date",          2 },   
                            { "shopArrivalDate",       3 }, 
                            { "storeCD",               6 },
                            { "supplierCD",            7 },
                            { "itemCD",                8 },
                            { "modelNumber",           9 },
                            { "productName",          10 },
                            { "colorCD",              11 },
                            { "sizeCD",               12 },
                            { "SKU",                  13 },
                            { "quantity",             14 },
                            { "suggestedRetailPrice", 15 },
                            { "cost",                 16 },
                        };
            Text.Csv.Convert(csv, table, colIndexes, false);
            return table;
        }
    }
}
