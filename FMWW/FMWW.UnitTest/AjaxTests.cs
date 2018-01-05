using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.UnitTest
{
    [TestClass]
    public class AjaxTests
    {
        [TestMethod]
        public void TestSnipError()
        {
            Assert.AreEqual(
                "商品数が30000を超えていますので、エクスポートができません。",
                FMWW.Core.Helpers.Ajax.SnipError("setError(\"商品数が30000を超えていますので、エクスポートができません。\");isError	= false;\n"));
        }
    }
}
