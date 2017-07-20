using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace FMWW.MdAnalysis.UnitTest
{
    [TestClass]
    public class CsvTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestCsv()
        {
            var page = new FMWW.MdAnalysis.PresentSales.Ref.Page()
                {
                    UserAccount = UserAccount,
                    PageContext = new FMWW.MdAnalysis.PresentSales.Ref.Context()
                    {
                        Date = new FMWW.Component.Between<DateTime>()
                        {
                            From = new DateTime(2014,1,1),
                            To = new DateTime(2014, 1, 1)
                        },
                        RankWidth = 1
                    }
                };
            Assert.AreEqual(System.IO.File.ReadAllText(@"..\..\data\20140101PresentSales.txt", Encoding.GetEncoding("shift_jis")), page.Csv());
        }

        [TestMethod]
        public void TestCsvOfSalesRanking()
        {
            var context = new SalesRanking.Ref.Context()
                {
                    StyleCode = "14FB5250"
                };
            context.Date.From = new DateTime(2014, 1, 1);
            context.Date.To = new DateTime(2014, 1, 1);
            var page = new FMWW.MdAnalysis.SalesRanking.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = context
            };
            Assert.AreEqual(System.IO.File.ReadAllText(@"..\..\data\salesRanking_20140101.csv", Encoding.GetEncoding("shift_jis")), page.Csv());
        }
    }
}
