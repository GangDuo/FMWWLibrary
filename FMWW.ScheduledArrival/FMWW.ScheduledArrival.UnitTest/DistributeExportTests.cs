using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace FMWW.ScheduledArrival.UnitTest
{
    [TestClass]
    public class DistributeExportTests
    {
        private static readonly Entity.UserAccount UserAccount = Entity.Factory.UserAccount.Load(".user.json");
        private static readonly FMWW.ScheduledArrival.DistributeExport.Ref.Context Context = 
            new FMWW.ScheduledArrival.DistributeExport.Ref.Context()
            {
                Code = "4444"
            };

        private static void Verify4444(string csv)
        {
            Assert.AreEqual(System.IO.File.ReadAllText(@"..\..\data\distribure-4444.csv", Encoding.GetEncoding("shift_jis")), csv);
        }

        [TestMethod]
        public void ExportDistributeWithCsv()
        {
            var page = new FMWW.ScheduledArrival.DistributeExport.Ref.Page()
            {
                UserAccount = UserAccount,
                PageContext = Context
            };
            Verify4444(page.Csv());
        }

        [TestMethod]
        public void ExportDistributeWithCsvAsync()
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                bool completed = false;
                var csv = String.Empty;

                var page = new FMWW.ScheduledArrival.DistributeExport.Ref.Page()
                {
                    UserAccount = UserAccount,
                    PageContext = Context
                };
                page.CsvDownloadCompleted += (result) =>
                {
                    csv = result;
                    completed = true;
                };
                page.CsvAsync();

                // Actual test code here.
                while (!completed)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                Debug.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
                return csv;
            }, System.Threading.Tasks.TaskCreationOptions.AttachedToParent);
            task.Wait();
            Verify4444(task.Result);
        }
    }
}
