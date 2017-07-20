using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMWW.Entity
{
    [DataContract]
    public class Item
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static DataTable CreateSimpleTable()
        {
            // table構造定義
            var table = new DataTable("Items");
            table.Columns.AddRange(new DataColumn[] {
                        new DataColumn("item_code", typeof(int)){ Caption = "アイテムコード" }, 
                        new DataColumn("item_name", typeof(String)){ Caption = "アイテム名" }, 
                    });
            table.PrimaryKey = new DataColumn[] { table.Columns["item_code"] };
            return table;
        }

        public static DataTable TranslateFrom(List<Item> items)
        {
            // table構造定義
            var table = CreateSimpleTable();

            // データ追加
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["item_code"] = item.Code;
                row["item_name"] = item.Name;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
