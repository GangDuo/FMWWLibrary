using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FMWW.Master.UnitTest
{
    [TestClass]
    public class ShelfExportTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void ExportShelf()
        {
            var page = new FMWW.Master.Shelf.Ref.Page()
                {
                    UserAccount = UserAccount
                };

            var csv = page.Csv();
            Assert.IsTrue(csv.Length > 0);
        }
    }
}
