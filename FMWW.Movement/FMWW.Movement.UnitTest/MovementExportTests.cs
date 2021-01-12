using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FMWW.Movement.UnitTest
{
    [TestClass]
    public class MovementExportTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestMethod1()
        {
            var context = new FMWW.Movement.Movement.MovementExport.Ref.Context()
            {
                MovementDate = new FMWW.Component.Between<System.Nullable<DateTime>>()
                {
                    From = new System.Nullable<DateTime>(DateTime.Parse("2021年1月12日")),
                    To = new System.Nullable<DateTime>(DateTime.Parse("2021年1月12日"))
                }
            };

            var page = new FMWW.Movement.Movement.MovementExport.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = context
            };
            var csv = page.Csv().Length;
            Assert.IsTrue(csv > 0);
        }
    }
}
