using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FMWW.Master.UnitTest
{
    [TestClass]
    public class ImageTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestOpenReadImage()
        {
            var page = new FMWW.Master.Image.Ref.Page()
            {
                UserAccount = UserAccount
            };
            using (var source = page.AsStreamBy("-9733179"))
            using (var destination = System.IO.File.Open("9733179.jpg", System.IO.FileMode.Create))
            {
                // ストリームのコピー
                source.CopyTo(destination);
            }
        }

        [TestMethod]
        public void TestDownloadImage()
        {
            var page = new FMWW.Master.Image.Ref.Page()
            {
                UserAccount = UserAccount
            };
            page.DownloadBy("-9733180", "9733180.jpg");
        }
    }
}
