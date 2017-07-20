using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.MdAnalysis.SalesRanking
{
    /*
     * 使い方
                    Csv(new Core.FMWW.MdAnalysis.SalesRanking.Ref.Context()
                        {
                            Date = new Core.Between<Core.Nullable<DateTime>>()
                            {
                                From = new Core.Nullable<DateTime>(DateTime.Parse("2015/10/1")),
                                To = new Core.Nullable<DateTime>(DateTime.Parse("2015/10/1"))
                            }
                        });
     */
    class Downloader
    {
        private void Excel(Ref.Context context)
        {
            //        private static void DownloadProductsAsFile(string filename, Core.FMWW.ExternalInterface.Products.Ref.Context context)
            {
                var client = new FMWW.Http.Client() { CookieContainer = new System.Net.CookieContainer() };
                var auth = new FMWW.Core.PC.Authentication();

                FMWW.Component.SignedInEventHandler onSignedIn = null;
                onSignedIn = new FMWW.Component.SignedInEventHandler(
                    (o, arg) =>
                    {
                        auth.SignedIn -= onSignedIn;

                        var page = new Ref.Page(client)
                        {
                            PageContext = context
                        };
                        Action onReached = null;
                        onReached = () =>
                        {
                            page.Reached -= onReached;
                            page.Search();
                        };
                        page.Reached += onReached;
                        page.ReachAsync();

                        Action<FMWW.Http.IPage> onGoneAway = null;
                        onGoneAway = (p) =>
                        {
                            // 検索結果一覧画面
                            page.GoneAway -= onGoneAway;
                            p.ExcelAsync();
                            Action<byte[]> onExcelDownloadCompleted = null;
                            onExcelDownloadCompleted = (b) =>
                            {
                                p.ExcelDownloadCompleted -= onExcelDownloadCompleted;
                                //                                        if ("gzip" == this.ResponseHeaders["Content-Encoding"])
                            };
                            p.ExcelDownloadCompleted += onExcelDownloadCompleted;
                        };
                        page.GoneAway += onGoneAway;

                        //                        System.Net.UploadValuesCompletedEventHandler onUploadValuesCompleted = null;
                        //                        onUploadValuesCompleted = (o, args) =>
                        //                            {
                        //                                client.UploadValuesCompleted -= onUploadValuesCompleted;
                        //                                var html = Encoding.UTF8.GetString(args.Result);
                        ////                                completed(html);
                        //                            };
                        //                        client.UploadValuesCompleted += onUploadValuesCompleted;
                        //                        client.UploadValuesAsync(UrlMainMenu, HttpMethod.Post, MainMenu.CreateDistributeExport().Translate());

                        //                        client.ExportDistributesAsync(context,
                        //                            (csv) =>
                        //                            {
                        //                                byte[] binary = client.DownloadProductMaster(context);
                        //                                using (var w = new BinaryWriter(File.OpenWrite(filename)))
                        //                                {
                        //                                    w.Write(binary);
                        //                                }
                        //                                Debug.WriteLine(csv);
                        //                                onCompleted(csv, userToken);
                        //                            });
                    });
                auth.SignedIn += onSignedIn;
                //client.SignInAsync();
            }
        }

        private void Csv(Ref.Context context)
        {
            {
                var client = new FMWW.Http.Client() { CookieContainer = new System.Net.CookieContainer() };
                var auth = new FMWW.Core.PC.Authentication();
                FMWW.Component.SignedInEventHandler onSignedIn = null;
                onSignedIn = new FMWW.Component.SignedInEventHandler(
                    (o, arg) =>
                    {
                        auth.SignedIn -= onSignedIn;

                        var page = new Ref.Page(client)
                        {
                            PageContext = context
                        };

                        Action onReached = null;
                        onReached = () =>
                        {
                            page.Reached -= onReached;
                            page.CsvAsync();
                        };
                        page.Reached += onReached;
                        page.ReachAsync();

                        Action<string> onCsvDownloadCompleted = null;
                        onCsvDownloadCompleted = (csv) =>
                        {
                            page.CsvDownloadCompleted -= onCsvDownloadCompleted;
                            Debug.WriteLine(csv);
                        };
                        page.CsvDownloadCompleted += onCsvDownloadCompleted;
                    });
                auth.SignedIn += onSignedIn;
                //client.SignInAsync();
            }
        }
    }
}
