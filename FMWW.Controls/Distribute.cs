using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Controls
{
    public class Distribute
    {
        // 投入表番号
        public string Code { get; set; }

        // 作成日付
        public DateTime CreationDate { get; set; }

        // 更新日付
        public DateTime ModifiedDate { get; set; }

        // 掛計上日付
        public DateTime RecordingDate { get; set; }

        // 物流入荷予定日付
        public DateTime ExpectedWarehouseDeliveryDate { get; set; }

        // 店舗入荷予定日付 yyyy年mm月下旬
        public string ExpectedShopDeliveryDate { get; set; }

        // 担当者名
        public string Owner { get; set; }

        // 状態
        public string State { get; set; }

        // 仕入先名
        public string Vendor { get; set; }

        // 部門
        public string Branch { get; set; }

        // 品番
        public string ModelNumber { get; set; }

        // 品名
        public string ProductName { get; set; }
    }
}
