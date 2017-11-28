using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FMWW.Movement.UnitTest
{
    [TestClass]
    public class PurchasingExpensesTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestExcel()
        {
            var context = new FMWW.Movement.Movement.PurchasingExpenses.Ref.Context();
            context.MovementDate.From = new DateTime(2017, 11, 1);
            context.MovementDate.To = new DateTime(2017, 11, 10);
            context.Location.To = "9998";

            var page = new FMWW.Movement.Movement.PurchasingExpenses.Ref.Page()
                {
                    UserAccount = UserAccount,
                    PageContext = context
                };
            var bin = page.Excel();
            using (var fs = new System.IO.FileStream(@"out.xls", System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                fs.Write(bin, 0, bin.Length);
            }
            Assert.IsTrue(bin.Length > 0);
        }
    }
}
