using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FMWW.Entity
{
    public class Stock
    {
        public static DataTable CreateSimpleTable()
        {
            var table = new DataTable("Stock");
            var sku = new DataColumn("sku", typeof(String)) { Caption = "SKU" };
            var store = new DataColumn("store_code", typeof(String)) { Caption = "店舗コード" };
            table.Columns.AddRange(
                new DataColumn[] {
                    sku,
                    store,
                    new DataColumn("quantity", typeof(Int32)){ Caption = "数量" }
                });
            table.PrimaryKey = new DataColumn[] { sku, store };
            return table;
        }
    }
}
