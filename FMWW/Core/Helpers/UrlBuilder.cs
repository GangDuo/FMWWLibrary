using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FMWW.Core.Helpers
{
    public class UrlBuilder
    {
        public static Uri Build(string absPath)
        {
            var ub = new UriBuilder(Uri.UriSchemeHttps, Core.AbstractAuthentication.HostName) { Path = absPath };
            return ub.Uri;
        }

        public static Uri BuildContentsUrl(string path)
        {
            return Core.Helpers.UrlBuilder.Build("/JMODE_ASP/faces/contents/" + Regex.Replace(path, @"^/*", ""));
        }

        public static Uri GetImageUrlBy(string productCode)
        {
            var ub = new UriBuilder(Uri.UriSchemeHttps, Core.AbstractAuthentication.HostName)
            {
                Path = "/JMODE_ASP/faces/contents/imageServlet",
                Query = String.Format(@"style={0}&id=0&dir=system", productCode)
            };
            return ub.Uri;
        }
    }
}
