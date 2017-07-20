using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FMWW.ScheduledArrival.DistributeExport.Ref
{
    public class Distribute
    {
        public static readonly string CLM_ID = "id";
        public static readonly string CLM_INDEX = "index";
        public static readonly string CLM_WH_SCHE_DATE = "wh_sche_date";
        public static readonly string CLM_SHOPARRIVAL_DATE = "shopArrival_date";
        public static readonly string CLM_PAYMENT_DATE = "payment_date";
        public static readonly string CLM_FROM = "from";
        public static readonly string CLM_TO = "to";
        public static readonly string CLM_SUPPLIER_CODE = "supplier_code";
        public static readonly string CLM_ITEM_CODE = "item_code";
        public static readonly string CLM_MODEL_NUMBER = "model_number";
        public static readonly string CLM_PRODUCT_NAME = "product_name";
        public static readonly string CLM_COLOR_CODE = "color_code";
        public static readonly string CLM_SIZE_CODE = "size_code";
        public static readonly string CLM_SKU = "sku";
        public static readonly string CLM_QUANTITY = "quantity";
        public static readonly string CLM_RETAIL_PRICE = "retail_price";
        public static readonly string CLM_DEALER_PRICE = "dealer_price";
        public static readonly string CLM_CREATION_DATE = "creation_date";
        public static readonly string CLM_MODIFIED_DATE = "modified_date";
        public static readonly string CLM_DELETION_DATE = "deletion_date";
        public static readonly string CLM_FLAG = "flag";
        public static readonly string CLM_GROSS_QUANTITY = "gross_quantity";
        public static readonly string CLM_COST_PRICE = "cost_price";
        public static readonly string CLM_SEGMENT = "segment";
        public static readonly string CLM_OWNER = "owner";
        public static readonly string CLM_STATUS = "status";

        public static DataTable CreateSimpleTable()
        {
            // table構造定義
            var table = new DataTable("Distributes");
            table.Columns.AddRange(new DataColumn[] {
                        new DataColumn(CLM_ID, typeof(String)){ Caption = "伝票番号" },
                        new DataColumn(CLM_INDEX, typeof(String)){ Caption = "伝票行番号" },
                        new DataColumn(CLM_WH_SCHE_DATE, typeof(String)){ Caption = "入荷予定日" },
                        new DataColumn(CLM_SHOPARRIVAL_DATE, typeof(String)){ Caption = "店舗納品予定日" },
                        new DataColumn(CLM_PAYMENT_DATE, typeof(String)){ Caption = "掛計上日" },
                        new DataColumn(CLM_FROM, typeof(String)){ Caption = "出荷元" },
                        new DataColumn(CLM_TO, typeof(String)){ Caption = "店舗" },
                        new DataColumn(CLM_SUPPLIER_CODE, typeof(String)){ Caption = "仕入先" },
                        new DataColumn(CLM_ITEM_CODE, typeof(String)){ Caption = "アイテム" },
                        new DataColumn(CLM_MODEL_NUMBER, typeof(String)){ Caption = "品番" },
                        new DataColumn(CLM_PRODUCT_NAME, typeof(String)){ Caption = "品名" },
                        new DataColumn(CLM_COLOR_CODE, typeof(String)){ Caption = "色コード" },
                        new DataColumn(CLM_SIZE_CODE, typeof(String)){ Caption = "サイズコード" },
                        new DataColumn(CLM_SKU, typeof(String)){ Caption = "SKU" },
                        new DataColumn(CLM_QUANTITY, typeof(String)){ Caption = "数量" },
                        new DataColumn(CLM_RETAIL_PRICE, typeof(String)){ Caption = "上代単価" },
                        new DataColumn(CLM_DEALER_PRICE, typeof(String)){ Caption = "下代単価" },
                        new DataColumn(CLM_CREATION_DATE, typeof(String)){ Caption = "作成日付" },
                        new DataColumn(CLM_MODIFIED_DATE, typeof(String)){ Caption = "修正日付" },
                        new DataColumn(CLM_DELETION_DATE, typeof(String)){ Caption = "削除日付" },
                        new DataColumn(CLM_FLAG, typeof(String)){ Caption = "更新フラグ" },
                        new DataColumn(CLM_GROSS_QUANTITY, typeof(String)){ Caption = "総数量" },
                        new DataColumn(CLM_COST_PRICE, typeof(String)){ Caption = "仕入単価" },
                        new DataColumn(CLM_SEGMENT, typeof(String)){ Caption = "商品区分" },
                        new DataColumn(CLM_OWNER, typeof(String)){ Caption = "担当者" },
                        new DataColumn(CLM_STATUS, typeof(String)){ Caption = "状態" },
                    });
            table.PrimaryKey = new DataColumn[] { table.Columns["id"], table.Columns["index"] };
            return table;
        }

        public static void InsertRow(DataTable table, string[] values)
        {
            foreach (var v in values)
            {
                var row = table.NewRow();
                row[Distribute.CLM_ID] = v;
                table.Rows.Add(row);
            }
        }
    }
}
