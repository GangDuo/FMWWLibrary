using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FMWW.Master.UnitTest
{
    [TestClass]
    public class PriceTagTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");

        [TestMethod]
        public void TestUploadPriceTag()
        {
            var filename = Path.GetTempFileName();
            var lines = new string[]
            {
                "コード,名称,,",
                Path.GetFileNameWithoutExtension(filename) + ",209,,",
                "バーコード,枚数,品名上段,品名下段",
                "0010000070279,1,,",
                "0010000070453,1,,",
                "0010000070514,1,,"
            };

            using (var sw = new StreamWriter(
                filename,
                false,
                System.Text.Encoding.GetEncoding("shift_jis")))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
            var uploader = new FMWW.Master.PriceTag.New.Page()
                {
                    UserAccount = UserAccount,
                    PathShiftJis = filename
                };
            var canExecute = uploader.CanExecute(null);
            Assert.IsTrue(canExecute);
            if (!canExecute)
            {
                return;
            }
            uploader.Execute(null);
            Assert.AreEqual("インポートに成功しました。", uploader.ResultMessage);
        }
    }
}
