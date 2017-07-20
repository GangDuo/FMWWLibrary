using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FMWW.Entity
{
    public class Distribute
    {
        public static DataTable CreateTemporaryTable()
        {
            var table = new DataTable();
            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("distributeNo"         , typeof(UInt64)){Caption="投入表番号"},
                new DataColumn("branchid"             , typeof(UInt32 )){Caption="伝票行番号"},
                new DataColumn("wh_sche_date"         , typeof(DateTime)){Caption=""},
                new DataColumn("shopArrivalDate"      , typeof(DateTime)){Caption="店舗納品予定日"},
                new DataColumn("storeCD"              , typeof(String)){Caption="店舗コード"},
                new DataColumn("supplierCD"           , typeof(String)){Caption="仕入先"},
                new DataColumn("itemCD"               , typeof(Int32 )){Caption="アイテム"},
                new DataColumn("modelNumber"          , typeof(String)){Caption="品番"},
                new DataColumn("productName"          , typeof(String)){Caption="品名"},
                new DataColumn("colorCD"              , typeof(String)){Caption="色コード"},
                new DataColumn("sizeCD"               , typeof(String)){Caption="サイズコード"},
                new DataColumn("SKU"                  , typeof(String)){Caption="TODO:jmode商品マスタのバーコード1に対応させる"},
                new DataColumn("quantity"             , typeof(Decimal)){Caption="投入数量"},
                new DataColumn("suggestedRetailPrice" , typeof(Decimal)){Caption="上代単価"},
                new DataColumn("cost"                 , typeof(Decimal)){Caption="下代単価"},
            });
            //table.PrimaryKey = new DataColumn[] { table.Columns["code"] };
            return table;
        }

    }
}
