using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Forms.Controls.UnitTest
{
    /// <summary>
    /// UnitTest1 の概要の説明
    /// </summary>
    [TestClass]
    public class UriBuilderTests
    {
        public UriBuilderTests()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private static readonly string SchemeName = "https";
        private static readonly string HostName = FMWW.Core.Config.Instance.HostName;

        [TestMethod]
        public void TestUriBuilder1()
        {
            string uriString = String.Format("{0}://{1}/JMODE_ASP/faces/index.jsp?time=", SchemeName, HostName);
            var expected = new Uri(uriString);
            var actual = new UriBuilder(SchemeName, HostName)
            {
                Path = "/JMODE_ASP/faces/index.jsp",
                Query = "time="
            }.Uri;
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(uriString, actual.ToString());
        }
    }
}
