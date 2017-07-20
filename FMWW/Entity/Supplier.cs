using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMWW.Entity
{
    [DataContract]
    public class Supplier
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static DataTable TranslateFrom(List<Supplier> suppliers)
        {
            // table構造定義
            var table = new DataTable("Suppliers");
            table.Columns.AddRange(new DataColumn[] {
                        new DataColumn("supplier_code", typeof(String)), 
                        new DataColumn("supplier_name", typeof(String)), 
                    });
            table.PrimaryKey = new DataColumn[] { table.Columns["supplier_code"] };

            // データ追加
            foreach (var supplier in suppliers)
            {
                var row = table.NewRow();
                row["supplier_code"] = supplier.Code;
                row["supplier_name"] = supplier.Name;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
