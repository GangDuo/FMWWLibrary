using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMWW.Controls
{
    //[DataContract(Name = "filter")]
    [DataContract]
    public class Filter
    {
        // 直営店
        public const string SC_CHAIN_STORE = "01";
        // 物流倉庫
        public const string SC_WAREHOUSE = "02";
        // FC店
        public const string SC_FRANCHISE = "03";

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public HashSet<string> Seasons { get; set; }

        [DataMember(Name = "brands")]
        public HashSet<string> Brands { get; set; }

        [DataMember(Name = "lines")]
        public HashSet<string> Lines { get; set; }
        public HashSet<string> Items { get; set; }

        [DataMember(Name = "shopCode")]
        public string ShopCode { get; set; }

        [DataMember(Name = "shopClass")]
        public string ShopClass { get; set; }
    }

    //[DataContract(Name="filters")]
    //[Serializable]
    //public class Filters
    //{
    //    private List<Filter> filers;
    //    public Filters() {
    //        filers = new List();
    //        persons.Add(new Person("Bob", "Human"));
    //        persons.Add(new Person("Tom", "Jackson"));
    //        persons.Add(new Person("Jack", "Hamilton"));
    //    }
    //    [DataMember(Name="personsList")]
    //    public IList PersonsList 
    //    { 
    //        get {            return persons;        }
    //        set {            persons = value.ToList();
    //        }
}
