using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Http
{
    public class HTMLParser
    {
        private mshtml.HTMLDocument _document = new mshtml.HTMLDocument();
        public mshtml.HTMLDocument Document
        {
            get { return _document; }
        }

        public HTMLParser(string text)
        {
            mshtml.IHTMLDocument2 htmldoc2 = _document as mshtml.IHTMLDocument2;
            htmldoc2.write(new object[] { text });
        }
    }
}
