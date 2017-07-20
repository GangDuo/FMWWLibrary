using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.UnitTest
{
    [TestClass]
    public class UrlBuilderTests
    {
        [TestMethod]
        public void TestBuild()
        {

            Assert.AreEqual(
                new UriBuilder(Uri.UriSchemeHttps, FMWW.Core.AbstractAuthentication.HostName)
                {
                    Path = "/JMODE_ASP/Import"
                }.ToString(),
                FMWW.Core.Helpers.UrlBuilder.Build("/JMODE_ASP/Import").ToString());
            
            Assert.AreEqual(
                new UriBuilder(Uri.UriSchemeHttps, FMWW.Core.AbstractAuthentication.HostName)
                {
                    Path = "/JMODE_ASP/faces/contents/index.jsp"
                }.ToString(),
                FMWW.Core.MainMenu.Url.ToString());
        }
    }
}
