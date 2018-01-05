using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace FMWW.ExternalInterface.UnitTest
{
    [TestClass]
    public class DownloadProductMasterTest
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");
        private static readonly FMWW.ExternalInterface.Products.Ref.Context Context = new FMWW.ExternalInterface.Products.Ref.Context() { Barcode = "0000001002560" };

        private static void Verify0000001002560(string filename)
        {
            using (var excel = new NetOffice.ExcelApi.Application())
            {
                var workBook = excel.Workbooks.Open(filename);
                var workSheet = (NetOffice.ExcelApi.Worksheet)workBook.Worksheets[1];

                Assert.AreEqual("reading.test", workSheet.Cells[2, 1].Value);
                Assert.AreEqual("読み込みテスト", workSheet.Cells[2, 2].Value);

                excel.Quit();
            }
        }

        [TestMethod]
        public void TestExcel()
        {
            var p = new FMWW.ExternalInterface.Products.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = Context
            };
            var filename = System.IO.Path.GetTempFileName();
            Util.FileSystem.WriteBinary(filename, p.Excel());
            Verify0000001002560(filename);
            System.IO.File.Delete(filename);
        }

        [TestMethod]
        public void TestExcelAsync()
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                bool completed = false;
                byte[] bin = null;
                var p = new FMWW.ExternalInterface.Products.Ref.Page()
                {
                    UserAccount = UserAccount,
                    PageContext = Context
                };
                p.ExcelDownloadCompleted += (binary) =>
                {
                    bin = binary;
                    completed = true;
                };
                p.ExcelAsync();

                // Actual test code here.
                while (!completed)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                return bin;
            }, System.Threading.Tasks.TaskCreationOptions.AttachedToParent);
            task.Wait();
            var filename = System.IO.Path.GetTempFileName();
            Util.FileSystem.WriteBinary(filename, task.Result);
            Verify0000001002560(filename);
            System.IO.File.Delete(filename);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CanNotExportOverLimit()
        {
            var page = new FMWW.ExternalInterface.Products.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = new FMWW.ExternalInterface.Products.Ref.Context()
                {
                    ItemCode = "221"
                }
            };
            var binary = page.Excel();
        }
    }
}
