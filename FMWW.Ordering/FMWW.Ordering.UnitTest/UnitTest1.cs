using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FMWW.Ordering.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestMethod1()
        {
            var context = new Shop.Ref.Context();
            context.OrderDate.From = new DateTime(2014, 1, 1);
            context.OrderDate.To = new DateTime(2014, 1, 1);
            var page = new FMWW.Ordering.Shop.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = context
            };
            Assert.AreEqual(System.IO.File.ReadAllText(@"..\..\data\Order_20140101.csv", Encoding.GetEncoding("shift_jis")), page.Csv());
        }
    }
}
