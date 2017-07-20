using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FMWW.Master.UnitTest
{
    [TestClass]
    public class SupplierExportTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestExcel()
        {
            var page = new FMWW.Master.Supplier.Ref.Page()
            {
                UserAccount = UserAccount
            };
            var csv = page.Csv();
            Assert.IsTrue(csv.Length > 0);
        }
    }
}
