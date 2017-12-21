using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.Master.Supplier.Ref.Detail
{
    class ReplicaPage
    {
        public string Raw { get; private set; }
        public Dictionary<string, string> Supplier { get; private set; }

        public ReplicaPage(string html)
        {
            Raw = html;
            Supplier = new Dictionary<string, string>();
            var parser = new FMWW.Http.HTMLParser(html);
            mshtml.HTMLDocument document = parser.Document;
            mshtml.IHTMLElementCollection inputs = document.getElementsByTagName("input");
            foreach (mshtml.IHTMLElement elm in inputs)
            {
                string title = elm.getAttribute("title") ?? "";
                var id = elm.id ?? "";
                if ((title.Length == 0) || (id.Length == 0))
                {
                    continue;
                }
                string value = elm.getAttribute("value");
                Supplier.Add(title, value);
                Trace.WriteLine(String.Format("{0} {1} {2}", elm.id, title, value));

                try
                {
                    var input = elm as mshtml.IHTMLInputElement;
                    var suffix = ":select";
                    var t = title + suffix;
                    var v = "";
                    mshtml.IHTMLElementCollection spans = document.getElementById(elm.id + suffix).children;
                    foreach (mshtml.IHTMLElement span in spans)
                    {
                        if (span.getAttribute("value") == input.value)
                        {
                            v = span.getAttribute("text");
                            continue;
                        }
                    }
                    Supplier.Add(t, v);
                    Trace.WriteLine(String.Format("{0} {1} {2}", elm.id + suffix, t, v));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
            mshtml.IHTMLElement remark = document.getElementById("remark");
            Supplier.Add("備考", remark.innerText);
        }
    }
}
