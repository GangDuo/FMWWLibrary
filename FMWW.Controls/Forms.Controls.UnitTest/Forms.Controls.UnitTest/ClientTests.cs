using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;

namespace Forms.Controls.UnitTest
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void TestGenDistributeInScheduledArrival()
        {
            var ins = new System.Collections.Generic.Dictionary<string, int>(){
                {"001", 4},
                {"002", 8},
            };
            System.Diagnostics.Debug.WriteLine(Codeplex.Data.DynamicJson.Serialize(ins));
            /*
            var json = @"{""QuantityPerShop"": [
                {""Key"": ""001"" , ""Value"": 4}, 
                {""Key"": ""002"" , ""Value"": 3}, 
                {""Key"": ""004"" , ""Value"": 3}, 
                {""Key"": ""005"" , ""Value"": 3}, 
                {""Key"": ""006"" , ""Value"": 3}, 
                {""Key"": ""007"" , ""Value"": 3}, 
                {""Key"": ""008"" , ""Value"": 3}, 
                {""Key"": ""009"" , ""Value"": 3}, 
                {""Key"": ""010"" , ""Value"": 3}, 
                {""Key"": ""011"" , ""Value"": 3}, 
                {""Key"": ""012"" , ""Value"": 3}, 
                {""Key"": ""013"" , ""Value"": 3}, 
                {""Key"": ""014"" , ""Value"": 3}, 
                {""Key"": ""015"" , ""Value"": 3}, 
                {""Key"": ""016"" , ""Value"": 3}, 
                {""Key"": ""017"" , ""Value"": 3}, 
                {""Key"": ""018"" , ""Value"": 3}, 
                {""Key"": ""019"" , ""Value"": 3}, 
                {""Key"": ""020"" , ""Value"": 3}, 
                {""Key"": ""021"" , ""Value"": 3}, 
                {""Key"": ""022"" , ""Value"": 3}, 
                {""Key"": ""023"" , ""Value"": 3}, 
                {""Key"": ""024"" , ""Value"": 3}, 
                {""Key"": ""1101"", ""Value"": 3}, 
                {""Key"": ""1201"", ""Value"": 3}, 
                {""Key"": ""1301"", ""Value"": 3}, 
                {""Key"": ""1401"", ""Value"": 3}, 
                {""Key"": ""1501"", ""Value"": 3}, 
                {""Key"": ""2001"", ""Value"": 3}, 
                {""Key"": ""2002"", ""Value"": 3}, 
                {""Key"": ""2003"", ""Value"": 3}, 
                {""Key"": ""2004"", ""Value"": 3}, 
                {""Key"": ""2005"", ""Value"": 3}
            ]}";*/
            var json = @"{""QuantityPerShop"": {
                ""001"": 4, 
                ""002"": 3, 
                ""004"": 3, 
                ""005"": 3, 
                ""006"": 3, 
                ""007"": 3, 
                ""008"": 3, 
                ""009"": 3, 
                ""010"": 3, 
                ""011"": 3, 
                ""012"": 3, 
                ""013"": 3, 
                ""014"": 3, 
                ""015"": 3, 
                ""016"": 3, 
                ""017"": 3, 
                ""018"": 3, 
                ""019"": 3, 
                ""020"": 3, 
                ""021"": 3, 
                ""022"": 3, 
                ""023"": 3, 
                ""024"": 3
            }}";

            ins = new System.Collections.Generic.Dictionary<string, int>();
            dynamic obj = Codeplex.Data.DynamicJson.Parse(json);
            var a = System.Text.RegularExpressions.Regex.Replace(obj.QuantityPerShop.ToString(), @"(^{|}$)", "");
            foreach (var item in a.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                var key = System.Text.RegularExpressions.Regex.Replace(pair[0], @"(^""|""$)", "");
                var value = Int32.Parse(pair[1]);
                ins.Add(key, value);
            }
            System.Diagnostics.Debug.WriteLine(obj.QuantityPerShop.ToString());
        }
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "BrandCode")]
        public string BrandCode { get; set; }

        [DataMember(Name = "ItemCode")]
        public string ItemCode { get; set; }

        [DataMember(Name = "PaymentDate")]
        public string PaymentDate { get; set; }

        [DataMember(Name = "SeasonCode")]
        public string SeasonCode { get; set; }

        [DataMember(Name = "ShopCodes")]
        public string[] ShopCodes { get; set; }

        [DataMember(Name = "ShopScheDate")]
        public string[] ShopScheDate { get; set; }

        [DataMember(Name = "SupplierCode")]
        public string[] SupplierCode { get; set; }
        
        [DataMember(Name = "WhScheDate")]
        public string[] WhScheDate { get; set; }
    }

    [DataContract]
    public class Product
    {
        [DataMember(Name = "Code")]
        public string Code { get; set; }

        [DataMember(Name = "Jan")]
        public string Jan { get; set; }

        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "SizeCode")]
        public string SizeCode { get; set; }

/*
            "QuantityPerShop": {
                "001": 4, 
                "002": 3, 
                "004": 3, 
                "005": 3, 
                "006": 3, 
                "007": 3, 
                "008": 3, 
                "009": 3, 
                "010": 3, 
                "011": 3, 
                "012": 3, 
                "013": 3, 
                "014": 3, 
                "015": 3, 
                "016": 3, 
                "017": 3, 
                "018": 3, 
                "019": 3, 
                "020": 3, 
                "021": 3, 
                "022": 3, 
                "023": 3, 
                "024": 3, 
                "1101": 3, 
                "1201": 3, 
                "1301": 3, 
                "1401": 3, 
                "1501": 3, 
                "2001": 3, 
                "2002": 3, 
                "2003": 3, 
                "2004": 3, 
                "2005": 3
            }, 
            "": "99"
 * */
    }
}
