using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using T = Text;
using F = FMWW;

namespace FMWW.Controls
{
    public class MainMenu
    {
        public int Menu { get; set; }
        public int TabIndex { get; set; }
        public int SubMenu { get; set; }
        public int SubMenuItem { get; set; }

        public MainMenu()
        {
            this.TabIndex = 0;
        }
    }
    public enum Menu : int
    {
        MdAnalysis = 5,
        Master = 9,
    }
    public enum SubMenuInMdAnalysis : int
    {
        /* 店舗売上集計 */
        GrossStoreSales = 6,
    }
    public enum SubMenuInMaster : int
    {
        /* Cポップ印刷 */
        PrintPopAdvertising = 22,
    }

    public class Client
    {
        public Queue<string> DistributeCodes { get; private set; }

        private Controls.NonDispBrowser Browser = new Controls.NonDispBrowser();
        private Queue<WebBrowserDocumentCompletedEventHandler> _documentCompletedEventHandler = new Queue<WebBrowserDocumentCompletedEventHandler>();

        private static readonly string FeatureControl = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
        private static readonly string SchemeName = "https";
        private static readonly string HostName = Core.Config.Instance.HostName;
        
        private static string FeatureBrowserEmulation()
        {
            return (new StringBuilder(FeatureControl)).Append("FEATURE_BROWSER_EMULATION").ToString();
        }

        private static string FeatureDocumentCompatibleMode()
        {
            return (new StringBuilder(FeatureControl)).Append("FEATURE_DOCUMENT_COMPATIBLE_MODE").ToString();
        }

        public static void Initialize(string assemblyName)
        {
            Microsoft.Win32.Registry.SetValue(FeatureBrowserEmulation(), assemblyName, 8000, RegistryValueKind.DWord);
            Microsoft.Win32.Registry.SetValue(FeatureDocumentCompatibleMode(), assemblyName, 80000, RegistryValueKind.DWord);
        }

        public static void Finalize(string assemblyName)
        {
            using(var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
                key.DeleteValue(assemblyName);
            }
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_DOCUMENT_COMPATIBLE_MODE", true))
            {
                key.DeleteValue(assemblyName);
            }
        }

        public Client(MainMenu menu)
        {
            Browser.AddDocumentCompletedEventHandler(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
            {
                // ダミー
                Debug.WriteLine(args.Url.ToString());
            }));

            Browser.AddDocumentCompletedEventHandler(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
            {
                var pattern = "^" + new UriBuilder(SchemeName, HostName)
                {
                    Path = "/JMODE_ASP/faces/index.jsp",
                    Query = "time="
                }.Uri.ToString().Replace(".", @"\.").Replace("?", @"\?");
                if (!Regex.IsMatch(args.Url.ToString(), pattern))
                {
                    return;
                }

                Debug.WriteLine("ログイン");
                var doc = (sender as WebBrowser).Document;
                foreach (HashSet<string> entries in F.Profile.Admin.UserAccount.Conf.Keys)
                {
                    foreach (string entry in entries)
                    {
                        Debug.WriteLine(String.Format("{0}:{1}", entry, F.Profile.Admin.UserAccount.Conf[entries]));
                        if (null != doc.GetElementById(entry))
                        {
                            doc.GetElementById(entry).SetAttribute("value", F.Profile.Admin.UserAccount.Conf[entries]);
                            break;
                        }
                    }
                }
                var id = "form1:login";
                if (null != doc.GetElementById(id))
                {
                    doc.GetElementById(id).InvokeMember("click");
                    //this.Browser.RemoveDocumentCompletedEventHandler();
                    return;
                }
            }));

            Browser.AddDocumentCompletedEventHandler(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
            {
                if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/index.jsp" }.Uri.ToString() != args.Url.ToString())
                {
                    return;
                }

                Debug.WriteLine("メインメニュー");
                var doc = (sender as WebBrowser).Document;
                Debug.Assert("メインメニュー"  == doc.Title);
                var selectTab = "";
                if (0 != menu.TabIndex)
                {
                    selectTab = "  document.getElementById('menu:1').children[" + menu.TabIndex + "].click();";
                }
                var editor = new StringBuilder();
                editor.AppendLine("setTimeout(function(){");
                editor.AppendLine("  if (0 == document.getElementById('menu:0').children.length) {");
                editor.AppendLine("    setTimeout(arguments.callee, 1000);");
                editor.AppendLine("    return;");
                editor.AppendLine("  }");
                editor.AppendLine("  document.getElementById('menu:0').children[" + menu.Menu + "].click();");
                editor.AppendLine(selectTab);
                editor.AppendLine("  document.getElementById('menu:2').children[" + menu.SubMenu + "].childNodes[" + menu.SubMenuItem + "].click();");
                editor.AppendLine("}, 1000);");

                doc.InvokeScript("eval", new object[] { editor.ToString() });
                //this.Browser.RemoveDocumentCompletedEventHandler();
            }));
        }

        //public delegate void DocumentCompletedEventHandler(object sender, EventArgs e);
        public Client ContinueWith(WebBrowserDocumentCompletedEventHandler handler)
        {
            Browser.AddDocumentCompletedEventHandler(handler);
            return this;
        }

        public Client Run()
        {
            Browser.Navigate(new UriBuilder(SchemeName, HostName).Uri);
            return this;
        }

        // 入荷予定->投入表 入力
        public static Client GenDistributeInScheduledArrival(string json)
        {
            var script = "var d = " + json;
            return new Controls.Client(new Controls.MainMenu() { Menu = 1, SubMenu = 6, SubMenuItem = 1 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                  {
                      if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                      {
                          return;
                      }
                      
                      var editor = new StringBuilder();
                      editor.AppendLine("(function(){");
                      editor.AppendLine("  setTimeout(function(){");
                      editor.AppendLine("    if ((!document.getElementById('search_button').onclick)");
                      editor.AppendLine("     || (!document.getElementById('dest:dest:SELECT').childNodes)");
                      editor.AppendLine("     || (0 === document.getElementById('dest:dest:SELECT').childNodes.length)) {");
                      editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                      editor.AppendLine("      return;");
                      editor.AppendLine("    }");
                      editor.AppendLine("    var contains = function(xs, value){");
                      editor.AppendLine("        for(var i = 0; i < xs.length; i++){");
                      editor.AppendLine("            if(xs[i].toString() === value.toString()){");
                      editor.AppendLine("                return true;");
                      editor.AppendLine("            }");
                      editor.AppendLine("        }");
                      editor.AppendLine("        return false;");
                      editor.AppendLine("    };");
                      editor.AppendLine("    eval('" + script + "');");
                      editor.AppendLine("    document.getElementById('sup').value = d.SupplierCode;");
                      editor.AppendLine("    document.getElementById('categoryGp').value = d.ItemCode;");
                      editor.AppendLine("    document.getElementById('season').value = d.SeasonCode;");
                      editor.AppendLine("    document.getElementById('brand').value = d.BrandCode;");
                      editor.AppendLine("    /* shopSelect */");
                      editor.AppendLine("    for(var i = 0; i < document.getElementById('dest:dest:SELECT').childNodes.length; i++) {");
                      editor.AppendLine("      var shopCD = document.getElementById('dest:dest:SELECT').childNodes[i].value;");
                      editor.AppendLine("      if (contains(d.ShopCodes, shopCD)) {");
                      editor.AppendLine("        document.getElementById('dest:dest:SELECT').childNodes[i].setAttribute('selected', 'selected');");
                      editor.AppendLine("      }");
                      editor.AppendLine("    }");
                      editor.AppendLine("    document.getElementById('search_button').click();");
                      editor.AppendLine("  }, 1000);");
                      editor.AppendLine("})();");


                      var doc = (sender as WebBrowser).Document;
                      doc.InvokeScript("eval", new object[] { editor.ToString() });
                  }))
              .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_PRIENTRY.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }

                    var editor = new StringBuilder();
                    editor.AppendLine("(function(){");
                    editor.AppendLine("  setTimeout(function(){");
                    editor.AppendLine("    if (!document.getElementById('search_button').onclick) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");
                    editor.AppendLine("    if (0 === document.getElementById('list').children.length) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");
                    editor.AppendLine("    if (!document.getElementById('checkbox_0')) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");
                    editor.AppendLine("    if (!document.getElementById('checkbox_0').parentNode.onclick) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");
                    editor.AppendLine("    document.getElementById('checkbox_0').parentNode.click();");
                    editor.AppendLine("    document.getElementById('checkbox_0').parentNode.click();");

                    editor.AppendLine("    eval('" + script + "');");
                    editor.AppendLine("    /* 必要な品番を個別選択 */");
                    editor.AppendLine("    var dictionary = {};");
                    editor.AppendLine("    for (var i = 0; i < document.component[0].native.length; i++) {");
                    editor.AppendLine("        var j = 1;");
                    editor.AppendLine("        // 品番 サイズコード");
                    editor.AppendLine("        dictionary['' + document.component[0].native[i][1].native");
                    editor.AppendLine("                    +'' + document.component[0].native[i][4].native] = i;");
                    editor.AppendLine("        for (var j = 0; j < document.component[0].native[i].length; j++) {");
                    editor.AppendLine("        }");
                    editor.AppendLine("    }");
                    editor.AppendLine("    var indexOf = function(productCD, sizeCd) {");
                    editor.AppendLine("        return dictionary[productCD];");
                    editor.AppendLine("    };");
                    editor.AppendLine("    var selectProduct = function(index) {");
                    editor.AppendLine("        var div = document.createElement('div');");
                    editor.AppendLine("        div.innerHTML = '&nbsp;<'");
                    editor.AppendLine("                        + 'script defer=\"defer\">\\n'");
                    editor.AppendLine("                        + 'var httpObj = getHttpObj();'");
                    editor.AppendLine("                        + 'httpObj.open(\"POST\", \"../../../SlipList\", true);'");
                    editor.AppendLine("                        + 'httpObj.onreadystatechange = function(){'");
                    editor.AppendLine("                        + '  if(httpObj.readyState != 4){'");
                    editor.AppendLine("                        + '    return;'");
                    editor.AppendLine("                        + '  }else if(httpObj.readyState == 4 && httpObj.status == 200){'");
                    editor.AppendLine("                        + '    var	strResult	= httpObj.responseText;'");
                    editor.AppendLine("                        + '    var isError		= true;'");
                    editor.AppendLine("                        + '    try{'");
                    editor.AppendLine("                        + '      eval(strResult);'");
                    editor.AppendLine("                        + '    }catch(e){'");
                    editor.AppendLine("                        + '      isError = true;'");
                    editor.AppendLine("                        + '    }'");
                    editor.AppendLine("                        + '    if(isError){'");
                    editor.AppendLine("                        + '      alert(\"サーバーでの処理に失敗しました。メニュー画面に戻り再試行して下さい。\");'\n");
                    editor.AppendLine("                        + '      self.state = -1;'");
                    editor.AppendLine("                        + '    }else{'");
                    editor.AppendLine("                        + '      self.state --;'");
                    editor.AppendLine("                        + '    }'");
                    editor.AppendLine("                        + '    return;'");
                    editor.AppendLine("                        + '  }else if(this.httpObj.readyState == 4){'");
                    editor.AppendLine("                        + '    alert(\"HTTP_ERROR:\" + this.httpObj.status + \"サーバーとの通信が途絶しました。メニュー画面に戻り再試行して下さい。\");'");
                    editor.AppendLine("                        + '    self.state ++;'");
                    editor.AppendLine("                        + '    self.state = -1;'");
                    editor.AppendLine("                        + '  }'");
                    editor.AppendLine("                        + '};'");
                    editor.AppendLine("                        + 'httpObj.setRequestHeader(\"Content-Type\" , \"application/x-www-form-urlencoded; charset=UTF-8\");'");
                    editor.AppendLine("                        + 'httpObj.send(\"action=set&row=' + index + '&col=0&value=checked&index=0&cache=\" + (new Date()).getTime());'\n");
                    editor.AppendLine("                        + '</'");
                    editor.AppendLine("                        + 'script>';");
                    editor.AppendLine("    };");
                    editor.AppendLine("    for(var i = 0; i < d.Products.length; i++) {");
                    editor.AppendLine("        d.Products[i]['dspid'] = indexOf('' + d.Products[i]['Code'] + d.Products[i]['SizeCode']);");
                    editor.AppendLine("        selectProduct(d.Products[i]['dspid']);");
                    editor.AppendLine("    }");
                    editor.AppendLine("    d.Products.sort(");
                    editor.AppendLine("        function(a, b){");
                    editor.AppendLine("            var av = parseInt(a[\"dspid\"]);");
                    editor.AppendLine("            var bv = parseInt(b[\"dspid\"]);");
                    editor.AppendLine("            if( av < bv ) return -1;");
                    editor.AppendLine("            if( av > bv ) return 1;");
                    editor.AppendLine("            return 0;");
                    editor.AppendLine("        }");
                    editor.AppendLine("    );");
                    editor.AppendLine("    var setAttributes = function(element, attributes) {");
                    editor.AppendLine("      for (key in attributes){");
                    editor.AppendLine("        element.setAttribute(key, attributes[key]);");
                    editor.AppendLine("      }");
                    editor.AppendLine("    };");
                    editor.AppendLine("    var txtarea = (function() {");
                    editor.AppendLine("      var element = document.createElement('textarea');");
                    editor.AppendLine("      element.innerHTML = JSON.stringify(d);");
                    editor.AppendLine("      setAttributes(element, { id : 'pasteAreaFromExcel'");
                    editor.AppendLine("        , rows : 20");
                    editor.AppendLine("        , cols : 30");
                    editor.AppendLine("        , style: \"position: absolute; top: 50px; left: 1000px;\" });");
                    editor.AppendLine("      return element;");
                    editor.AppendLine("    })();");
                    editor.AppendLine("    document.body.appendChild(txtarea);");
                    editor.AppendLine("    setTimeout(function(){document.getElementById('search_button').click();}, 5000);\n");
                    editor.AppendLine("  }, 1000);");
                    editor.AppendLine("})();");

                    var doc = (sender as WebBrowser).Document;
                    doc.InvokeScript("eval", new object[] { editor.ToString() });
                }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_PRIENTRY_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var editor = new StringBuilder();
                        editor.AppendLine("(function(){");
                        editor.AppendLine("  setTimeout(function(){");
                        editor.AppendLine("    if (!document.getElementById('mat_div')) {");
                        editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        editor.AppendLine("      return;");
                        editor.AppendLine("    }");
                        editor.AppendLine("    if (!document.getElementById('mat_div').getElementsByTagName('div')[1]) {");
                        editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        editor.AppendLine("      return;");
                        editor.AppendLine("    }");
                        editor.AppendLine("    if (!document.getElementById('mat_div').getElementsByTagName('div')[1].getElementsByTagName('tr')[0]) {");
                        editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        editor.AppendLine("      return;");
                        editor.AppendLine("    }");
                        editor.AppendLine("    eval('" + script + "');");
                        editor.AppendLine("    /* 合計発注数量の入力 */");
                        editor.AppendLine("    var tds = document.getElementById('mat_div').getElementsByTagName('div')[1].getElementsByTagName('tr')[0].getElementsByTagName('td');");
                        editor.AppendLine("    for(var i = 0; i < tds.length; i++){");
                        editor.AppendLine("        var input = tds[i].getElementsByTagName('input')[3];");
                        editor.AppendLine("        input.value = d.Products[i]['Quantity'];");
                        editor.AppendLine("        input.parentNode.lastChild.innerHTML = d.Products[i]['Quantity'];");
                        editor.AppendLine("        input.updateSummaries();");
                        editor.AppendLine("    }");
                        editor.AppendLine("    /* 振分数入力 */");
                        editor.AppendLine("    var sortedStores = (function() {");
                        editor.AppendLine("        var stores = {};");
                        editor.AppendLine("        var trs = document.getElementById('mat_div').getElementsByTagName('div')[2].getElementsByTagName('tr');");
                        editor.AppendLine("        for(var row = 2; row < trs.length; row++) {");
                        editor.AppendLine("            var spans = trs[row].getElementsByTagName('span');");
                        editor.AppendLine("            stores[spans[0].innerHTML.substring(0, spans[0].innerHTML.indexOf(\" \"))] = row - 1;");
                        editor.AppendLine("        }");
                        editor.AppendLine("        return stores;");
                        editor.AppendLine("    })();");
                        editor.AppendLine("    var stores = (function() {");
                        editor.AppendLine("      var ret = [];");
                        editor.AppendLine("      var table = document.getElementById('2');");
                        editor.AppendLine("      var trs = table.getElementsByTagName('tr');");
                        editor.AppendLine("      for(var i = 2;i < trs.length;i++){");
                        editor.AppendLine("        var storeCD = trs[i].getElementsByTagName('span')[0].innerHTML.split(' ')[0];");
                        editor.AppendLine("        ret.push(storeCD);");
                        editor.AppendLine("      }");
                        editor.AppendLine("      return ret;");
                        editor.AppendLine("    })();");
                        editor.AppendLine("    var trs = document.getElementById('mat_div').getElementsByTagName('div')[3].getElementsByTagName('tr');");
                        editor.AppendLine("    for(var clm = 0; clm < d.Products.length; clm++) {");
                        editor.AppendLine("      for(var row = 2; row < trs.length; row++) {");
                        editor.AppendLine("          var inputs = trs[row].getElementsByTagName('input');");
                        editor.AppendLine("          var v = d.Products[clm]['QuantityPerShop'][stores[row - 2]];");
                        editor.AppendLine("          if(\"undefined\" === typeof v) {");
                        editor.AppendLine("              v = 0;");
                        editor.AppendLine("          }");
                        editor.AppendLine("          inputs[clm].value = v;");
                        editor.AppendLine("          inputs[clm].parentNode.childNodes[1].innerHTML = v;");
                        editor.AppendLine("          inputs[clm].updateSummaries();");
                        editor.AppendLine("      }");
                        editor.AppendLine("    }");
                        editor.AppendLine("    document.getElementById('btn:check').click();");

                        editor.AppendLine("    /* 店舗入荷予定日付 */");
                        editor.AppendLine("    setTimeout(function(){");
                        editor.AppendLine("      document.getElementById('shopArvScheDate').value = d.ShopScheDate;");
                        editor.AppendLine("      document.getElementById('shopArvScheDate').onblur();");
                        editor.AppendLine("      /* 物流入荷予定日付 */");
                        editor.AppendLine("      setTimeout(function(){");
                        editor.AppendLine("        document.getElementById('wareArvScheDate').value = d.WhScheDate;");
                        editor.AppendLine("        document.getElementById('wareArvScheDate').onblur();");
                        editor.AppendLine("        /* 掛計上日付 */");
                        editor.AppendLine("        setTimeout(function(){");
                        editor.AppendLine("          document.getElementById('paymentDate').value = d.PaymentDate;");
                        editor.AppendLine("          document.getElementById('paymentDate').onblur();");
                        editor.AppendLine("          alert('完了');");
                        editor.AppendLine("        }, 1000);");
                        editor.AppendLine("      }, 1000);");
                        editor.AppendLine("    }, 1000);");
                        editor.AppendLine("  }, 1000);");
                        editor.AppendLine("})();");

                        editor.AppendLine("/* 振分数量の入力と出力の差分チェック */");
                        editor.AppendLine("var onClick = function() {");
                        editor.AppendLine("  var page2Json = function() {");
                        editor.AppendLine("    var shops = (function(){");
                        editor.AppendLine("      /* 店舗コード一覧 */");
                        editor.AppendLine("      var shops = [];");
                        editor.AppendLine("      var table = document.getElementById('2');");
                        editor.AppendLine("      var trs = table.getElementsByTagName('tr');");
                        editor.AppendLine("      for (var row = 2; row < trs.length; row++) {");
                        editor.AppendLine("        var header = trs[row].getElementsByTagName('span')[0].innerHTML;");
                        editor.AppendLine("        var storeCode = header.split(' ')[0];");
                        editor.AppendLine("        shops.push(storeCode);");
                        editor.AppendLine("      }");
                        editor.AppendLine("      return shops;");
                        editor.AppendLine("    })();");

                        editor.AppendLine("    var QuantityPerShopOf = function(clm){");
                        editor.AppendLine("      var xs = {};");
                        editor.AppendLine("      var table = document.getElementById('3');");
                        editor.AppendLine("      var trs = table.getElementsByTagName('tr');");
                        editor.AppendLine("      for (var row = 2; row < trs.length; row++) {");
                        editor.AppendLine("        var q = trs[row].getElementsByTagName('td')[clm].getElementsByTagName('span')[0].innerHTML;");
                        editor.AppendLine("        xs[shops[row - 2]] = parseInt(q.replace(',', ''), 10);");
                        editor.AppendLine("      }");
                        editor.AppendLine("      return xs;");
                        editor.AppendLine("    };");

                        editor.AppendLine("    /* janと合計発注数 */");
                        editor.AppendLine("    var xs = [];");
                        editor.AppendLine("    var table = document.getElementById('1');");
                        editor.AppendLine("    var trs = table.getElementsByTagName('tr');");
                        editor.AppendLine("    var products = trs[0].getElementsByTagName('td');");
                        editor.AppendLine("    for (var i = 0; i < products.length; i++) {");
                        editor.AppendLine("      var spans = products[i].getElementsByTagName('span');");
                        editor.AppendLine("      var jan = spans[6].innerHTML;");
                        editor.AppendLine("      var q = spans[13].innerHTML;");
                        editor.AppendLine("      var x = {};");
                        editor.AppendLine("      x['Jan'] = jan;");
                        editor.AppendLine("      x['Quantity'] = parseInt(q.replace(',', ''), 10);");
                        editor.AppendLine("      x['QuantityPerShop'] = QuantityPerShopOf(i);");
                        editor.AppendLine("      xs.push(x);");
                        editor.AppendLine("    }");
                        editor.AppendLine("    return JSON.stringify(xs, null, \"\\t\");");
                        editor.AppendLine("  };");
                        editor.AppendLine("  var original = function() {");
                        editor.AppendLine("    var xs = [];");
                        editor.AppendLine("    eval('" + script + "');");
                        editor.AppendLine("    for (var i = 0; i < d.Products.length; i++) {");
                        editor.AppendLine("      var x = {};");
                        editor.AppendLine("      x.Jan = d.Products[i].Jan;");
                        editor.AppendLine("      x.Quantity = d.Products[i].Quantity;");
                        editor.AppendLine("      x.QuantityPerShop = d.Products[i].QuantityPerShop;");
                        editor.AppendLine("      xs.push(x);");
                        editor.AppendLine("    }");
                        editor.AppendLine("    return JSON.stringify(xs, null, \"\\t\");");
                        editor.AppendLine("  };");
                        editor.AppendLine("  var setAppearance = function(id, color) {");
                        editor.AppendLine("    var elm = document.getElementById(id);");
                        editor.AppendLine("    elm.style.backgroundColor = color;");
                        editor.AppendLine("    elm.style.color = 'rgb(255, 255, 255)';");
                        editor.AppendLine("  };");

                        editor.AppendLine("  var s1 = original();");
                        editor.AppendLine("  var s2 = page2Json();");
                        editor.AppendLine("  var color = 'rgb(0, 255, 0)';");
                        editor.AppendLine("  document.getElementById('mtx:in').innerText = s1;");
                        editor.AppendLine("  document.getElementById('mtx:out').innerText = s2;");
                        editor.AppendLine("  if(s1 != s2) {");
                        editor.AppendLine("    color = 'rgb(255, 0, 0)';");
                        editor.AppendLine("  }");
                        editor.AppendLine("  setAppearance('mtx:in', color);");
                        editor.AppendLine("  setAppearance('mtx:out', color);");
                        editor.AppendLine("};");
                        editor.AppendLine("(function() {");
                        editor.AppendLine("  /* Online Diff */");
                        editor.AppendLine("  var div = document.createElement('div');");
                        editor.AppendLine("  div.style.top = '100px';");
                        editor.AppendLine("  div.style.left = '1200px';");
                        editor.AppendLine("  div.style.position = 'absolute';");
                        editor.AppendLine("  div.innerHTML = '<form method=\"GET\" action=\"http://www.diffchecker.com/\" target=\"_blank\">'");
                        editor.AppendLine("                + '  <input style=\"height: 40px; width: 50px;\" type=\"SUBMIT\" value=\"Diff\"/>'");
                        editor.AppendLine("                + '</form>';");
                        editor.AppendLine("  document.body.appendChild(div);");
                        editor.AppendLine("})();");
                        editor.AppendLine("(function() {");
                        editor.AppendLine("  var div = document.createElement('div');");
                        editor.AppendLine("  div.style.top = '50px';");
                        editor.AppendLine("  div.style.left = '1200px';");
                        editor.AppendLine("  div.style.position = 'absolute';");
                        editor.AppendLine("  div.innerHTML = \"<input id=\\\"btn:check\\\" style=\\\"height: 40px; width: 50px;\\\" type=\\\"button\\\" onclick=\\\"onClick();\\\" value=\\\"Check\\\"/>\";");
                        editor.AppendLine("  document.body.appendChild(div);");
                        editor.AppendLine("})();");
                        editor.AppendLine("(function() {");
                        editor.AppendLine("  var div = document.createElement('div');");
                        editor.AppendLine("  div.style.top = '50px';");
                        editor.AppendLine("  div.style.left = '900px';");
                        editor.AppendLine("  div.style.position = 'absolute';");
                        editor.AppendLine("  div.innerHTML = '<textarea id=\"mtx:in\" rows=\"6\" cols=\"48\">'");
                        editor.AppendLine("                + '入力'");
                        editor.AppendLine("                + '</textarea>'");
                        editor.AppendLine("                + '<textarea id=\"mtx:out\" rows=\"6\" cols=\"48\">'");
                        editor.AppendLine("                + '出力'");
                        editor.AppendLine("                + '</textarea>';");
                        editor.AppendLine("  document.body.appendChild(div);");
                        editor.AppendLine("})();");

                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { editor.ToString() });
                    }));
        }

        // 入荷予定->投入表 照会
        public static Client RefDistributeInScheduledArrival(
            F.ScheduledArrival.Distribute.Ref.Context context,
            Action<List<Distribute>> completed = null)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 1, SubMenu = 6, SubMenuItem = 2 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }
                    //document.getElementById('list').getElementsByTagName('div')[1].getElementsByTagName('table')[0].getElementsByTagName('tr')
                    var editor = new StringBuilder();
                    editor.AppendLine("(function(){");
                    editor.AppendLine("  setTimeout(function(){");
                    editor.AppendLine("    document.getElementById('distribute_cd_from').value = " + context.Code.From + ";");
                    editor.AppendLine("    document.getElementById('distribute_cd_to').value = " + context.Code.To + ";");
                    editor.AppendLine("    document.getElementById('status').value = '';");
                    editor.AppendLine("    document.getElementById('search_button').click();");
                    editor.AppendLine("  }, 1000);");
                    editor.AppendLine("})();");

                    var doc = (sender as WebBrowser).Document;
                    doc.InvokeScript("eval", new object[] { editor.ToString() });
                }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }
                    var editor = new StringBuilder();
                    editor.AppendLine("(function(){");
                    editor.AppendLine("  setTimeout(function(){");
                    editor.AppendLine("    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");

                    editor.AppendLine("    if (2 !== document.getElementById('list').children.length) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");

                    //editor.AppendLine("    var div = document.getElementById('list').children[1];");
                    //editor.AppendLine("    var tr = div.getElementsByTagName('tr')[1];");
                    //editor.AppendLine("    var tds = tr.getElementsByTagName('td');");
                    //editor.AppendLine("    var array = [];");
                    //editor.AppendLine("    for(var i = 0; i < tds.length; ++i) {");
                    //editor.AppendLine("        array.push(tds[i].firstChild.innerHTML);");
                    //editor.AppendLine("    }");
                    editor.AppendLine("    window.external.Completed(JSON.stringify(document.component[0].native));");
                    editor.AppendLine("  }, 1000);");
                    editor.AppendLine("})();");
                    var doc = (sender as WebBrowser).Document;
                    doc.InvokeScript("eval", new object[] { editor.ToString() });

                    var browser = sender as NonDispBrowser;
                    browser.ObjectForScripting = new Notifier(new Action<string>(
                        (text) =>
                        {
                            Dictionary<string, string> properties = new Dictionary<string, string>()
                            {
                                {"投入表番号",       "Code"},
                                {"作成日付",         "CreationDate"},
                                {"更新日付",         "ModifiedDate"},
                                {"掛計上日付",       "RecordingDate"},
                                {"物流入荷予定日付", "ExpectedWarehouseDeliveryDate"},
                                {"店舗入荷予定日付", "ExpectedShopDeliveryDate"},
                                {"担当者",           "Owner"},
                                {"状態",             "State"},
                                {"仕入先",           "Vendor"},
                                {"部門",             "Branch"},
                                {"品番",             "ModelNumber"},
                                {"品名",             "ProductName"},
                            };
                            var ss = new List<Distribute>();
                            foreach (var xs in T.Json.Parse<List<List<Component>>>(text))
                            {
                                var d = new Distribute();
                                foreach (Component x in xs)
                                {
                                    var value = HttpUtility.HtmlDecode(x.Value);
                                    // プロパティの有無を確認する
                                    Type typeDistribute = typeof(Distribute);
                                    PropertyInfo pi = typeDistribute.GetProperty(properties[HttpUtility.HtmlDecode(x.Title)]);
                                    if (pi != null)
                                    {
                                        if (Type.GetTypeCode(pi.PropertyType) == Type.GetTypeCode(typeof(DateTime)))
                                        {
                                            pi.SetValue(d, DateTime.Parse(value), null);
                                        }
                                        else
                                        {
                                            pi.SetValue(d, value, null);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("プロパティがありません。");
                                    }
                                }
                                ss.Add(d);
                            }
                            if (null != completed)
                            {
                                completed(ss);
                            }
                        }));
                }))
                ;
        }

        // 入荷予定->投入表 修正
        public enum AccessPrivilege
        {
            Append = 1, // 入力
            Read,       // 照会
            Modify      // 修正
        }
        //public static Client EditDistributeInScheduledArrival(string code, Action printingCompleted)
        public static Client PrintDistribute(string code, Action printingCompleted, AccessPrivilege mode)
        {
            int SubMenuItem = (int)mode;
            var client = new Controls.Client(new Controls.MainMenu() { Menu = 1, SubMenu = 6, SubMenuItem = SubMenuItem })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                  {
                      if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                      {
                          return;
                      }
                      var editor = new StringBuilder();
                      editor.AppendLine("(function(){");
                      editor.AppendLine("  setTimeout(function(){");
                      editor.AppendLine("    var code = " + code + ";");
                      editor.AppendLine("    document.getElementById('distribute_cd_from').value = code;");
                      editor.AppendLine("    document.getElementById('distribute_cd_to').value = code;");
                      editor.AppendLine("    document.getElementById('search_button').click();");
                      editor.AppendLine("  }, 1000);");
                      editor.AppendLine("})();");

                      var doc = (sender as WebBrowser).Document;
                      doc.InvokeScript("eval", new object[] { editor.ToString() });
                  }))
                  .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                  {
                      if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                      {
                          return;
                      }
                      var editor = new StringBuilder();
                      editor.AppendLine("(function(){");
                      editor.AppendLine("  setTimeout(function(){");
                      editor.AppendLine("    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {");
                      editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                      editor.AppendLine("      return;");
                      editor.AppendLine("    }");
                      editor.AppendLine("    try {");
                      editor.AppendLine("    	var trs = document.getElementById('list').getElementsByTagName('div')[1].getElementsByTagName('table')[0].getElementsByTagName('tr');");
                      editor.AppendLine("    	trs[1].getElementsByTagName('td')[0].click();");
                      editor.AppendLine("    } catch(e) {");
                      editor.AppendLine("    }");
                      editor.AppendLine("  }, 1000);");
                      editor.AppendLine("})();");

                      var doc = (sender as WebBrowser).Document;
                      doc.InvokeScript("eval", new object[] { editor.ToString() });
                  }))
                  .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                  {
                      if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
                      {
                          return;
                      }
                      var statement = "document.getElementById('preview_button').click();";
                      if (AccessPrivilege.Read == mode)
                      {
                          statement = "window.external.Completed('');";
                          var browser = sender as NonDispBrowser;
                          browser.ObjectForScripting = new Notifier(new Action<string>(
                              (text) =>
                              {
                                  browser.Print();
                                  printingCompleted();
                              }));
                      }
                      var editor = new StringBuilder();
                      editor.AppendLine("(function(){");
                      editor.AppendLine("  setTimeout(function(){");
                      editor.AppendLine("    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {");
                      editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                      editor.AppendLine("      return;");
                      editor.AppendLine("    }");
                      //editor.AppendLine("    document.getElementById('preview_button').click();");
                      editor.AppendLine(statement);
                      editor.AppendLine("  }, 1000);");
                      editor.AppendLine("})();");

                      var doc = (sender as WebBrowser).Document;
                      doc.InvokeScript("eval", new object[] { editor.ToString() });
                  }));

            if (AccessPrivilege.Modify == mode)
            {
                client.ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                      {
                          if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_ENTRY.jsp" }.Uri.ToString() != args.Url.ToString())
                          {
                              return;
                          }
                          var browser = sender as NonDispBrowser;
                          var editor = new StringBuilder();
                          editor.AppendLine("(function(){");
                          editor.AppendLine("  setTimeout(function(){");
                          editor.AppendLine("    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {");
                          editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                          editor.AppendLine("      return;");
                          editor.AppendLine("    }");
                          editor.AppendLine("    window.external.Completed('');");
                          editor.AppendLine("  }, 1000);");
                          editor.AppendLine("})();");
                          browser.ObjectForScripting = new Notifier(new Action<string>(
                              (text) =>
                              {
                                  browser.Print();
                                  printingCompleted();
                              }));
                          var doc = browser.Document;
                          doc.InvokeScript("eval", new object[] { editor.ToString() });
                      }));
            }
            return client;
        }

        // ﾏｽﾀｰ->[商品ﾏｽﾀｰ]->ｱｲﾃﾑﾏｽﾀｰ 照会
        public static Client RefItems(Action<string> completed)
        {
            var client = new Controls.Client(new Controls.MainMenu() { Menu = 11, SubMenu = 25, SubMenuItem = 2 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var script = "(function(){"
                        + "  setTimeout(function(){"
                        + "    if (!document.getElementById('search_button').click) {\n"
                        + "      setTimeout(arguments.callee, 1000);\n"
                        + "      return;"
                        + "    }"
                        + "    document.getElementById('search_button').click();"
                        + "  }, 1000);"
                        + "})();";

                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { script });
                    }));

            client.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if ((new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/M081_CATEGORY_GP/M081_SELECT.jsp" }.Uri.ToString() != args.Url.ToString()) &&
                        (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/M081_CATEGORY_GP/M081_LIST.jsp" }.Uri.ToString() != args.Url.ToString()))
                    {
                        return;
                    }

                    var script = "(function(){"
                    + "  var items = [];"
                    + "  setTimeout(function(){"
                    + "    if ((!document.getElementById('SlipList:ALLDIV')) || (!document.getElementById('SlipList:ALLDIV').getElementsByTagName('table'))) {"
                    + "      setTimeout(arguments.callee, 1000);\n"
                    + "      return;"
                    + "    }"
                    + "    /* アイテム一覧 */"
                    + "    var table = document.getElementById('SlipList:ALLDIV').getElementsByTagName('table')[1];"
                    + "    var trs = table.getElementsByTagName('tr');"
                    + "    for(var i = 0; i < trs.length; i++) {"
                    + "      var spans = trs[i].getElementsByTagName('span');"
                    + "      var item = {\"code\":spans[0].innerText, \"name\":spans[1].innerText};"
                    + "      items.push(item);"
                    + "    }"
                    + "    /* 次ページヘ */"
                    + "    var inputs = document.getElementsByTagName('input');"
                    + "    for (var i = 0; i < inputs.length; i++) {"
                    + "      if ('submit' === inputs[i].type.toLowerCase()) {"
                    + "        if ('次ページへ' === inputs[i].value) {"
                    + "          if (!inputs[i].disabled) {"
                    + "            window.external.Completed(JSON.stringify(items));"
                    + "            inputs[i].click();"
                    + "            return;"
                    + "          } else {"
                    + "            /*console.log('finish');*/"
                    + "          }"
                    + "        }"
                    + "      }"
                    + "    }"
                    + "    window.external.Completed(JSON.stringify(items));"
                    + "    window.external.Completed('');"
                    + "  }, 1000);"
                    + "})();";

                    var browser = sender as NonDispBrowser;
                    browser.ObjectForScripting = new Notifier(new Action<string>(
                        (text) =>
                        {
                            completed(text);
                        }));
                    browser.Document.InvokeScript("eval", new object[] { script });
                });

            return client;
        }

        // ﾏｽﾀｰ->[各種ﾏｽﾀｰ]->仕入先ﾏｽﾀｰ 照会
        public static Client RefSuppliers(Action<string> completed)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 9, TabIndex = 1, SubMenu = 3, SubMenuItem = 2 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var script = "(function(){"
                        + "  setTimeout(function(){"
                        + "    if (!document.getElementById('search_button').click) {\n"
                        + "      setTimeout(arguments.callee, 1000);\n"
                        + "      return;"
                        + "    }"
                        + "    document.getElementById('search_button').click();"
                        + "  }, 1000);"
                        + "})();";

                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { script });
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/M040_SUPPLIER/M040_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var script = "(function(){"
                        + "  setTimeout(function(){"
                        + "    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {"
                        + "      setTimeout(arguments.callee, 1000);\n"
                        + "      return;"
                        + "    }"
                        + "    var suppliers = [];"
                        + "    var list = document.component[0].native;"
                        + "    for( var i = 0; i < list.length; i++) {"
                        + "      var supplier = {\"code\":list[i][0].native, \"name\":list[i][1].native};"
                        + "      suppliers.push(supplier);"
                        + "    }"
                        + "    window.external.Completed(JSON.stringify(suppliers));"
                        + "  }, 1000);"
                        + "})();";

                        var browser = sender as NonDispBrowser;
                        browser.ObjectForScripting = new Notifier(new Action<string>(
                            (text) =>
                            {
                                completed(text);
                            }));
                        browser.Document.InvokeScript("eval", new object[] { script });
                    }));
        }

        // ﾏｽﾀｰ->[商品ﾏｽﾀｰ]->Cポップ印刷 照会
        public static Client RefPrintPopAdvertising(string code, string title, Action printingCompleted)
        {
            if (null == code || 0 == code.Length)
            {
                throw new ArgumentException();
            }
            if (null == title || 0 == title.Length)
            {
                throw new ArgumentException();
            }

            return new Controls.Client(new Controls.MainMenu() { Menu = 11, SubMenu = 22, SubMenuItem = 1 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var browser = sender as NonDispBrowser;
                        var script = "(function(){"
                            + "  setTimeout(function(){"
                            + "    document.getElementById('form1:search_type').getElementsByTagName('input')[1].click();"
                            + "    document.getElementById('dest_cd1:dest').value = '001';"
                            + "    document.getElementById('code_from').value = '" + code + "';"
                            + "    document.getElementById('code_to').value = '" + code + "';"
                            + "    document.getElementById('search_button').click();"
                            + "  }, 1000);"
                            + "})();";
                        var doc = browser.Document;
                        doc.InvokeScript("eval", new object[] { script });
                        //browser.RemoveDocumentCompletedEventHandler();
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X045_160_CPOP_CATALOG/X045_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }
                        var browser = sender as NonDispBrowser;
                        var script = "(function(){"
                            + "  setTimeout(function(){"
                            + "    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {"
                            + "      setTimeout(arguments.callee, 1000);\n"
                            + "      return;"
                            + "    }"
                            + "    document.getElementById('search_button').click();"
                            + "  }, 1000);"
                            + "})();";
                        var doc = browser.Document;
                        doc.InvokeScript("eval", new object[] { script });
                        //browser.RemoveDocumentCompletedEventHandler();
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X045_160_CPOP_CATALOG/X045_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }
                        var browser = sender as NonDispBrowser;
                        var script = "(function(){"
                            + "  setTimeout(function(){"
                            + "    if ((!document.getElementById('loading')) || ('none' != document.getElementById('loading').style.display)) {"
                            + "      setTimeout(arguments.callee, 1000);\n"
                            + "      return;"
                            + "    }"
                            + "    window.external.Completed('');"
                            + "  }, 1000);"
                            + "})();";
                        browser.ObjectForScripting = new Notifier(new Action<string>(
                            (text) =>
                            {
                                browser.Print();
                                printingCompleted();
                            }));
                        var doc = browser.Document;
                        doc.Title = title;
                        doc.InvokeScript("eval", new object[] { script });
                        //browser.RemoveDocumentCompletedEventHandler();
                    }));
        }
#if false
        private struct EditableItem
        {
            public enum Type
            {
                Value,
                Native,
                CheckBox,
                ComboBox,
                TextArea,
            }

            //public string Id;
            public string Caption;
            public bool IsBranch;
            public Type EditableItemType;
            public uint Maxlength;

            public EditableItem(string caption, Type type = EditableItem.Type.Value, bool isBranch = true)
            {
                this.Caption = caption;
                this.IsBranch = isBranch;
                this.EditableItemType = type;
                this.Maxlength = 0;
            }
        }

        #region key:formのid、value:関連情報
        private static Dictionary<string, EditableItem> IdInfo = new Dictionary<string, EditableItem>()
        {
            // 品番側
            {"form1:style_nm",         new EditableItem("品名", EditableItem.Type.Value, false){Maxlength = 40}},
            {"form1:style_an",         new EditableItem("略称", EditableItem.Type.Value, false){Maxlength = 20}},
            {"form1:style_kn",         new EditableItem("カナ名", EditableItem.Type.Value, false){Maxlength = 40}},
            {"form1:style_rn",         new EditableItem("ローマ字名", EditableItem.Type.Value, false){Maxlength = 40}},
            {"supplier_list:SELECT",   new EditableItem("仕入先", EditableItem.Type.ComboBox, false){Maxlength = 5}},
            {"form1:mk_style_cd",      new EditableItem("メーカー品番", EditableItem.Type.Value, false){Maxlength = 30}},
            {"brand_list:SELECT",      new EditableItem("ブランド", EditableItem.Type.ComboBox, false){Maxlength = 3}},
            {"item_list:SELECT",       new EditableItem("サブアイテム", EditableItem.Type.ComboBox, false){Maxlength = 6}},
            {"season_list:SELECT",     new EditableItem("シーズン", EditableItem.Type.ComboBox, false){Maxlength = 6}},
            {"line_list:SELECT",       new EditableItem("ライン", EditableItem.Type.ComboBox, false){Maxlength = 3}},
            {"origin:SELECT",          new EditableItem("原産国", EditableItem.Type.ComboBox, false){Maxlength = 5}},
            {"delivery",               new EditableItem("納期", EditableItem.Type.Value, false){Maxlength = 10}},
            {"form1:style_attr1",      new EditableItem("品番属性1", EditableItem.Type.Value, false){Maxlength = 20}},
            {"form1:style_attr2",      new EditableItem("品番属性2", EditableItem.Type.Value, false){Maxlength = 20}},
            {"form1:style_attr3",      new EditableItem("品番属性3", EditableItem.Type.Value, false){Maxlength = 20}},
            {"form1:sample_code",      new EditableItem("サンプルコード", EditableItem.Type.Value, false){Maxlength = 12}},
            // combobox                
            {"plan:SELECT",            new EditableItem("企画区分", EditableItem.Type.ComboBox, false){Maxlength = 10}},
            {"material:SELECT",        new EditableItem("素材区分", EditableItem.Type.ComboBox, false){Maxlength = 15}},
            {"wear:SELECT",            new EditableItem("服種区分", EditableItem.Type.ComboBox, false){Maxlength = 10}},
            {"intention:SELECT",       new EditableItem("商品区分", EditableItem.Type.ComboBox, false){Maxlength = 15}},
            {"disable_cd:SELECT",      new EditableItem("使用可能CD", EditableItem.Type.ComboBox, false){Maxlength = 2}},
            // checkbox                
            {"form1:except_sales",     new EditableItem("売上除外区分", EditableItem.Type.CheckBox, false)},
            {"form1:except_analyse",   new EditableItem("分析除外区分", EditableItem.Type.CheckBox, false)},
            {"form1:novelty_flg",      new EditableItem("販促商品", EditableItem.Type.CheckBox, false)},
            {"form1:exchange_flg",     new EditableItem("交換商品", EditableItem.Type.CheckBox, false)},
            // textarea                
            {"form1:style_remark",     new EditableItem("備考", EditableItem.Type.TextArea, false){Maxlength = 300}},
            // SKU側                   
            {"form1:barcode",          new EditableItem("バーコード"){Maxlength = 20}},
            {"form1:barcode2",         new EditableItem("バーコード2"){Maxlength = 20}},
            {"form1:barcode3",         new EditableItem("バーコード3"){Maxlength = 20}},
            {"form1:barcode4",         new EditableItem("バーコード4"){Maxlength = 20}},
            {"form1:product_nm",       new EditableItem("SKU名"){Maxlength = 70}},
            {"form1:mk_product_cd",    new EditableItem("メーカーSKU"){Maxlength = 20}},
            {"form1:mk_color",         new EditableItem("メーカー色"){Maxlength = 10}},
            {"form1:mk_size",          new EditableItem("メーカーサイズ"){Maxlength = 5}},
            {"form1:product_attr1",    new EditableItem("属性1"){Maxlength = 40}},
            {"form1:product_attr2",    new EditableItem("属性2"){Maxlength = 40}},
            {"form1:product_attr3",    new EditableItem("属性3"){Maxlength = 40}},
            // native
            {"form1:retail_price",     new EditableItem("上代", EditableItem.Type.Native)},
            {"form1:sale_price",       new EditableItem("セール上代", EditableItem.Type.Native)},
            {"form1:buying_price",     new EditableItem("仕入単価", EditableItem.Type.Native)},
            {"form1:cost",             new EditableItem("原価", EditableItem.Type.Native)},
            {"form1:mk_price",         new EditableItem("メーカー上代", EditableItem.Type.Native)},
            {"form1:point_rt_price",   new EditableItem("ポイント上代", EditableItem.Type.Native)},
            {"form1:cleaning_point",   new EditableItem("交換ポイント", EditableItem.Type.Native)},
            // checkbox                
            {"form1:check_sale",       new EditableItem("セール", EditableItem.Type.CheckBox)},
            {"form1:check_standard",   new EditableItem("定番", EditableItem.Type.CheckBox)},
            {"form1:check_consign",    new EditableItem("委託", EditableItem.Type.CheckBox)},
            {"form1:check_sample",     new EditableItem("サンプル", EditableItem.Type.CheckBox)},
            {"form1:check_netinfo",    new EditableItem("ネット掲載", EditableItem.Type.CheckBox)},
            {"form1:check_management", new EditableItem("在庫を管理しない", EditableItem.Type.CheckBox)},
            {"form1:check_point",      new EditableItem("ポイント付与しない", EditableItem.Type.CheckBox)},
            // combobox                
            {"color1_list:SELECT",     new EditableItem("色", EditableItem.Type.ComboBox){Maxlength = 4}},
            {"color2_list:SELECT",     new EditableItem("色2", EditableItem.Type.ComboBox){Maxlength = 4}},
            {"size_list:SELECT",       new EditableItem("サイズ", EditableItem.Type.ComboBox){Maxlength = 4}},
        };
        #endregion

        private static string[] Split(string text, char[] separator)
        {
            var ss = new List<string>();
            var startIndex = 0;
            while (true)
            {
                var index = text.IndexOfAny(separator, startIndex);
                if (-1 == index)
                {
                    ss.Add(text.Substring(startIndex));
                    break;
                }
                else
                {
                    var length = index - startIndex;
                    if (0 < length)
                    {
                        ss.Add(text.Substring(startIndex, length));
                        ss.Add("\\r");
                    }
                    startIndex = index + 1;
                }
            }
            return ss.ToArray();
            
        }

        // ﾏｽﾀｰ->[商品ﾏｽﾀｰ]->商品登録 修正
        public static Client EditProducts(DataRow row, Action<string> completed, Action<string>  terminated)
        {
            string sku = row["SKU"].ToString();
            var vv = new Dictionary<string, string>();
            foreach (var key in IdInfo.Keys)
            {
                try
                {
                    var fieldName = IdInfo[key].Caption;
                    vv.Add(key, row[fieldName].ToString());
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            // 編集する項目のid一覧
            var ids = (from r in vv select r.Key).ToArray();
            // idの関連情報
            var info = (from r in IdInfo
                     where ids.Contains(r.Key)
                     select r)
                     .ToDictionary(source => source.Key, source => source.Value);
            var trunkEditor = new StringBuilder("");// 品番側
            var branchEditor = new StringBuilder("");// SKU側
            foreach (var key in info.Keys)
            {
                if (info[key].Maxlength > 0)
                {
                    int ilenb = Encoding.GetEncoding(932).GetByteCount(vv[key]);
                    if (info[key].Maxlength < ilenb)
                    {
                        return new Controls.Client(new Controls.MainMenu() { Menu = 9, SubMenu = 12, SubMenuItem = 3 })
                            .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                            {
                                var script = "(function(){"
                                           + "  window.external.Completed('');"
                                           + "})();";

                                var browser = sender as NonDispBrowser;
                                browser.ObjectForScripting = new Notifier(new Action<string>(
                                    (text) =>
                                    {
                                        completed(String.Format("{0} {1}長すぎ", sku, info[key].Caption));
                                    }));
                                browser.Document.InvokeScript("eval", new object[] { script });
                            }));
                    }
                }

                var editor = trunkEditor;
                // editorの選択
                if (info[key].IsBranch)
                {
                    editor = branchEditor;
                }
        
                // type別にscript記述
                switch (info[key].EditableItemType)
                {
                    case EditableItem.Type.Value:
                        editor.AppendLine(String.Format("if(document.getElementById('{0}').value !== '{1}')", key, vv[key]));
                        editor.AppendLine(String.Format("  document.getElementById('{0}').value = '{1}';{2}", key, vv[key], Environment.NewLine));
                        break;
                    case EditableItem.Type.Native:
                        var digit = int.Parse(vv[key],  System.Globalization.NumberStyles.AllowThousands);
                        editor.AppendLine(String.Format("if(document.getElementById('{0}').native !== '{1}')", key, digit));
                        editor.AppendLine(String.Format("  document.getElementById('{0}').native = '{1}';{2}", key, digit, Environment.NewLine));
                        break;
                    case EditableItem.Type.CheckBox:
                        var format = "document.getElementById('{0}').checked = {1};";
                        switch(vv[key])
                        {
                            case "1":
                                editor.AppendLine(String.Format(format, key, "true"));
                                break;
                            case "0":
                                editor.AppendLine(String.Format(format, key, "false"));
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    case EditableItem.Type.ComboBox:
                        var prefix = key.Split(new char[] { ':' })[0];
                        editor.AppendLine("(function() {");
                        editor.AppendLine("  var sku = '" + sku + "';");
                        editor.AppendLine("  var prefix = '" + prefix + "';");
                        editor.AppendLine("  var id = '" + key + "';");
                        editor.AppendLine("  var value = '" + vv[key] + "';");
                        editor.AppendLine("  if(contains(document.getElementById(id), value) || value.length === 0) {");
                        editor.AppendLine("    document.getElementById(id).value = value;");
                        editor.AppendLine("    document.getElementById(prefix + ':FEELD').value = value;");
                        editor.AppendLine("    return;");
                        editor.AppendLine("  }");
                        editor.AppendLine("  message += (sku + '\\n' + prefix + ' : ' + value + ' 存在しないコードです！\\n');");
                        editor.AppendLine("})();");
                        break;
                    case EditableItem.Type.TextArea:
                        editor.AppendLine("(function() {");
                        editor.AppendLine("  var removeChildren = function(node) {");
                        editor.AppendLine("    for (var i = node.childNodes.length - 1; i >= 0; --i) {");
                        editor.AppendLine("      node.removeChild(node.childNodes[i]);");
                        editor.AppendLine("    }");
                        editor.AppendLine("  };");

                        var ss = "'" + String.Join("','", Split(vv["form1:style_remark"], new char[] { '\r', '\n' })) + "'";
                        editor.AppendLine("  var ts = [" + ss + "];");
                        editor.AppendLine("  var xs = [];");
                        editor.AppendLine("  for(var i = 0; i < ts.length; ++i) {");
                        editor.AppendLine("   xs.push(document.createTextNode(ts[i]));");
                        editor.AppendLine("  }");

                        editor.AppendLine("  var ID = '" + key + "';");
                        editor.AppendLine("  removeChildren(document.getElementById(ID));");
                        editor.AppendLine("  for(var i = 0; i < xs.length; ++i) {");
                        editor.AppendLine("    document.getElementById(ID).appendChild(xs[i]);");
                        editor.AppendLine("  }");
                        editor.AppendLine("})();");
                        break;
                    default:
                        break;
                }
            }
#if false
            // {id : field_name}
            var d = new Dictionary<string, string>(){
                // 品番側
                {"form1:style_nm",       "品名"},
                {"form1:style_an",       "略称"},
                {"form1:style_kn",       "カナ名"},
                {"form1:style_rn",       "ローマ字名"},
                {"supplier_list:SELECT", "仕入先"},
                {"form1:mk_style_cd",    "メーカー品番"},
                {"brand_list:SELECT",    "ブランド"},
                {"item_list:SELECT",     "サブアイテム"},
                {"season_list:SELECT",   "シーズン"},
                {"line_list:SELECT",     "ライン"},
                {"origin:SELECT",        "原産国"},
                {"delivery",             "納期"},
/*
                {"form1:style_attr1",    "属性１"},
                {"form1:style_attr2",    "属性２"},
                {"form1:style_attr3",    "属性３"},
 */
                {"form1:sample_code",    "サンプルコード"},
                {"plan:SELECT",          "企画区分"},
                {"material:SELECT",      "素材区分"},
                {"wear:SELECT",          "服種区分"},
                {"intention:SELECT",     "商品区分"},
                {"disable_cd:SELECT",    "使用可能CD"},
                {"form1:except_sales",   "売上除外品"},
                {"form1:except_analyse", "分析除外品"},
                {"form1:novelty_flg",    "販促商品"},
                {"form1:exchange_flg",   "交換商品"},
                {"form1:style_remark",   "備考"},
                // SKU側
                {"form1:barcode",        "バーコード"},
                {"form1:barcode2",       "バーコード2"},
                {"form1:barcode3",       "バーコード3"},
                {"form1:barcode4",       "バーコード4"},
                {"form1:product_nm",     "SKU名"},
                {"form1:mk_product_cd",  "メーカーSKU"},
                {"form1:mk_color",       "メーカー色"},
                {"form1:mk_size",        "メーカーサイズ"},
                {"form1:retail_price",   "上代"},
                {"form1:sale_price",     "セール上代"},
                {"form1:buying_price",   "仕入単価"},
                {"form1:cost",           "原価"},
                {"form1:mk_price",       "メーカー上代"},
                {"form1:point_rt_price", "ポイント上代"},
                {"form1:cleaning_point", "交換ポイント"},
                {"form1:product_attr1",  "属性1"},
                {"form1:product_attr2",  "属性2"},
                {"form1:product_attr3",  "属性3"},
                // checkbox
                {"form1:check_sale",     "セール"},
                {"form1:check_standard", "定番"},
                {"form1:check_consign",  "委託"},
                {"form1:check_sample",   "サンプル"},
                {"form1:check_netinfo",  "ネット掲載"},
//document.getElementById('form1:check_sample').checked = true;
                // combobox
                {"color1_list:SELECT", "色"},
                {"color2_list:SELECT", "色2"},
                {"size_list:SELECT",   "サイズ"},
            };
            // {id : value}
            var v = new Dictionary<string, string>();
            foreach (var key in d.Keys)
            {
                try
                {
                    var fieldName  = d[key];
                    v.Add(key, row[fieldName].ToString());
                }
                catch (Exception e)
                {
                }
            }
#endif
            var globalEditor = new StringBuilder();
            globalEditor.AppendLine("var contains = function(selectNode, value) {");
            globalEditor.AppendLine("  var options = selectNode.getElementsByTagName('option');");
            globalEditor.AppendLine("  for(var i = 0; i < options.length; ++i) {");
            globalEditor.AppendLine("    if(options[i].value === value) {");
            globalEditor.AppendLine("      return true;");
            globalEditor.AppendLine("    }");
            globalEditor.AppendLine("  }");
            globalEditor.AppendLine("  return false;");
            globalEditor.AppendLine("};");
#if false
            var trunkEditor = new StringBuilder("");// 品番側
            var branchEditor = new StringBuilder("");// SKU側
            if (v.Keys.Contains("plan:SELECT"))
            {
                trunkEditor.AppendLine("var value = '" + v["plan:SELECT"] + "'");
                trunkEditor.AppendLine("if(contains(document.getElementById('plan:SELECT'), value)) {");
                trunkEditor.AppendLine("  document.getElementById('plan:SELECT').value = value;");
                trunkEditor.AppendLine("}");
            }

            if (v.Keys.Contains("form1:retail_price"))
            {
                branchEditor.AppendLine("      document.getElementById(ID_TXT_RETAIL_PRICE).native = '" + v["form1:retail_price"] + "';");
            }
            if (v.Keys.Contains("form1:buying_price"))
            {
                branchEditor.AppendLine("      document.getElementById('form1:buying_price').native = '" + v["form1:buying_price"] + "';");
            }
            if (v.Keys.Contains("form1:cost"))
            {
                branchEditor.AppendLine("      document.getElementById('form1:cost').native = '" + v["form1:cost"] + "';");
            }
            if (v.Keys.Contains("form1:product_attr1"))
            {
                branchEditor.AppendLine("      document.getElementById('form1:product_attr1').value = '" + v["form1:product_attr1"] + "';");
            }
#endif
            return new Controls.Client(new Controls.MainMenu() { Menu = 9, SubMenu = 12, SubMenuItem = 3 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var scriptEditor = new StringBuilder();
                        scriptEditor.AppendLine("(function(){");
                        scriptEditor.AppendLine("  setTimeout(function(){");
                        scriptEditor.AppendLine("    if (!document.getElementById('search_button').click) {");
                        scriptEditor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        scriptEditor.AppendLine("      return;");
                        scriptEditor.AppendLine("    }");
                        scriptEditor.AppendLine("    var sku = '" + sku + "';");
                        scriptEditor.AppendLine("    document.getElementById('form1:product_cd').value = sku;");
                        scriptEditor.AppendLine("    document.getElementById('form1:product_cd2').value = sku;");
                        scriptEditor.AppendLine("    document.getElementById('search_button').click();");
                        scriptEditor.AppendLine("  }, 1000);");
                        scriptEditor.AppendLine("})();");

                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { scriptEditor.ToString() });
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var script = "(function(){"
                                   + "  setTimeout(function(){"
                                   + "    if ((!document.getElementById('SlipList:COL_0')) && (0 === document.getElementById('SlipList:LIMIT_CAUTION').innerHTML.length)) {"
                                   + "      setTimeout(arguments.callee, 1000);\n"
                                   + "      return;"
                                   + "    }"
                                   + "    if(document.getElementById('SlipList:COL_0')) {"
                                   + "      document.getElementById('SlipList:COL_0').firstChild.click();"
                                   + "      return;"
                                   + "    }"
                                   + "    var sku = '" + sku + "';"
                                   + "    window.external.Completed(sku + ':表示するデータがありません。\\n');"
                                   + "  }, 1000);"
                                   + "})();"
                                   + "window.alert = function(){};";

                        var browser = sender as NonDispBrowser;
                        browser.ObjectForScripting = new Notifier(new Action<string>(
                            (text) =>
                            {
                                completed(text);
                            }));
                        browser.Document.InvokeScript("eval", new object[] { script });
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
                        {
                            return;
                        }

                        var scriptEditor = new StringBuilder();
                        scriptEditor.AppendLine("(function(){");
                        scriptEditor.AppendLine("  var message = '';");
                        scriptEditor.AppendLine(globalEditor.ToString());
                        scriptEditor.AppendLine("  setTimeout(function(){");
                        scriptEditor.AppendLine("    var ID_TXT_RETAIL_PRICE = \"form1:retail_price\";");
                        scriptEditor.AppendLine("    var ID_BTN_UPDATE = \"listentry_001:insetThis\";");
                        scriptEditor.AppendLine("    var ID_BTN_SUBMIT = \"form1:register\";");
                        scriptEditor.AppendLine("    ");
                        scriptEditor.AppendLine(trunkEditor.ToString());
                        scriptEditor.AppendLine("    var sku = '" + sku + "';");
                        scriptEditor.AppendLine("    var items = document.getElementById('listentry_001:mainlist').getElementsByTagName('tr');");
                        scriptEditor.AppendLine("    for(var i = 0; i < items.length; ++i) {");
                        scriptEditor.AppendLine("      var label = items[i].children[1].firstChild.innerHTML;");
                        scriptEditor.AppendLine("      if(sku !== label) {");
                        scriptEditor.AppendLine("        continue;");
                        scriptEditor.AppendLine("      }");
                        scriptEditor.AppendLine("    ");
//                        scriptEditor.AppendLine("      var btn = items[i].children[32].getElementsByTagName('input')[1];");
                        scriptEditor.AppendLine("      var btn = document.getElementById('listentry_001:input:' + i + ':LINEREINPUT');");
                        scriptEditor.AppendLine("      btn.click();");
                        scriptEditor.AppendLine("    ");
                        scriptEditor.AppendLine(branchEditor.ToString());
                        scriptEditor.AppendLine("      if(0 < message.length) {");
                        scriptEditor.AppendLine("        window.external.Completed(message);");
                        scriptEditor.AppendLine("        return;");
                        scriptEditor.AppendLine("      }");
                        scriptEditor.AppendLine("      document.getElementById(ID_BTN_UPDATE).click();");
                        scriptEditor.AppendLine("    }");
                        scriptEditor.AppendLine("    setTimeout(function() {");
                        scriptEditor.AppendLine("      if(isFixMode != 0) {");
                        scriptEditor.AppendLine("        setTimeout(arguments.callee, 1000);");
                        scriptEditor.AppendLine("        return;");
                        scriptEditor.AppendLine("      }");
                        scriptEditor.AppendLine("      document.getElementById(ID_BTN_SUBMIT).click();");
                        scriptEditor.AppendLine("    }, 1000);");
                        scriptEditor.AppendLine("  }, 1000);");
                        scriptEditor.AppendLine("})();");

                        var browser = sender as NonDispBrowser;
                        browser.ObjectForScripting = new Notifier(new Action<string>(
                            (text) =>
                            {
                                completed(text);
                            }))
                            {
                                OnTerminated = new Action<string>(v =>
                                {
                                    if (null == terminated)
                                    {
                                        return;
                                    }
                                    terminated(v);
                                })
                            };
                        browser.Document.InvokeScript("eval", new object[] { scriptEditor.ToString() });
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
                        // dummy
                    }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                    {
//                        if ("/JMODE_ASP/faces/contents/M130_PRODUCT/M130_MODIFY.jsp" != args.Url.ToString())
                        {
//                            return;
                        }

                        var scriptEditor = new StringBuilder();
                        // 登録結果取得
                        scriptEditor.AppendLine("(function(){");
                        scriptEditor.AppendLine("  setTimeout(function() {");
                        scriptEditor.AppendLine("    var sku = '" + sku + "';");
                        scriptEditor.AppendLine("    var cautionString = '';");
                        scriptEditor.AppendLine("    var items = document.getElementById('listentry_001:mainlist').getElementsByTagName('tr');");
                        scriptEditor.AppendLine("    for(var i = 0; i < items.length; ++i) {");
                        scriptEditor.AppendLine("      var msg = items[i].lastChild.firstChild.innerHTML;");
                        scriptEditor.AppendLine("      if(msg.length === 0) {");
                        scriptEditor.AppendLine("        continue;");
                        scriptEditor.AppendLine("      }");
                        scriptEditor.AppendLine("      cautionString += (sku + '->>>\\n' + msg + '\\n');");
                        scriptEditor.AppendLine("    }");
                        scriptEditor.AppendLine("    window.external.Completed(cautionString);");
                        scriptEditor.AppendLine("    window.external.Terminated(document.getElementById('form1:style_cd').value);");
                        scriptEditor.AppendLine("  }, 1000);");
                        scriptEditor.AppendLine("})();");

                        var browser = sender as NonDispBrowser;
                        browser.Document.InvokeScript("eval", new object[] { scriptEditor.ToString() });
                    }));
        }
#endif

        // 店舗管理 -> 店舗売上入力リスト[照会]
        public static Client RefJournalsInStoreManagement(Action<string> onCompleted, F.ForShop.Work.Journals.Ref.Context context = null)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 4, SubMenu = 7, SubMenuItem = 1 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }

                    var yesterday = DateTime.Today.AddDays(-1.0).ToString("yyyy/MM/dd");
                    var fromDate = context == null ? yesterday : context.PeriodOfSales.From.Value.ToString("yyyy/MM/dd");
                    var toDate = context == null ? yesterday : context.PeriodOfSales.To.Value.ToString("yyyy/MM/dd");
                    var storeCode = context == null ? "" : context.StoreCode;
                    var editor = new StringBuilder();
                    editor.AppendLine("(function(){");
                    editor.AppendLine("  function $(id) {");
                    editor.AppendLine("    return document.getElementById(id);");
                    editor.AppendLine("  }");

                    editor.AppendFormat("  var date_001 = '{0}';", fromDate);
                    editor.AppendFormat("  var date_002 = '{0}';", toDate);
                    editor.AppendFormat("  var storeCode = '{0}';", storeCode);
                    editor.AppendLine("  setTimeout(function(){");
                    editor.AppendLine("    if ((!$('search_button').click) ||");
                    editor.AppendLine("        (!$('date_001')) ||");
                    editor.AppendLine("        (!$('date_002')) ||");
                    editor.AppendLine("        (!$('form1:ps')) || (!$('form1:radio1')) ||");
                    editor.AppendLine("        (!$('form1:mode'))) {");
                    editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                    editor.AppendLine("      return;");
                    editor.AppendLine("    }");

                    editor.AppendLine("    $('date_001').value = date_001;");
                    editor.AppendLine("    $('date_002').value = date_002;");
                    editor.AppendLine("    date_001_value = date_001;");
                    editor.AppendLine("    date_002_value = date_002;");
                    // P&S
                    editor.AppendLine("    $('form1:ps').getElementsByTagName('input')[2].click();");
                    // 店舗
                    editor.AppendLine("    value_attr003 = storeCode;");
                    editor.AppendLine("    $('attr003:SELECT').value = storeCode;");
                    editor.AppendLine("    $('attr003:FEELD').value  = storeCode;");

                    // 明細単位
                    editor.AppendLine("    $('form1:radio1').getElementsByTagName('input')[1].click();");
                    editor.AppendLine("    $('search_button').click();");
                    editor.AppendLine("  }, 1000);");
                    editor.AppendLine("})();");


                    var doc = (sender as WebBrowser).Document;
                    doc.InvokeScript("eval", new object[] { editor.ToString() });
                }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/S085_SHOP_SALE_REPORT/S085_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }

                    var editor = new StringBuilder();
                    if ((context != null) && context.AsCsv)
                    {
                        editor.AppendLine(JsSalesTransWithCSV(context));
                    }
                    else
                    {
                        editor.AppendLine("(function(){");
                        editor.AppendLine("  setTimeout(function(){");
                        editor.AppendLine("    document.getElementById('excel_button').click();");
                        editor.AppendLine("    setTimeout(function(){");
                        editor.AppendLine("      if ('none' === document.getElementById('hiddener').getElementsByTagName('div')[1].style.display) {");
                        editor.AppendLine("        setTimeout(arguments.callee, 10000);");
                        editor.AppendLine("        return;");
                        editor.AppendLine("      }");
                        editor.AppendLine("      var url = document.getElementById('hiddener').getElementsByTagName('a')[0].href;");
                        editor.AppendLine("      window.external.Completed(url);");
                        editor.AppendLine("    }, 1000);");
                        editor.AppendLine("  }, 10000);");
                        editor.AppendLine("})();");
                    }

                    var browser = sender as WebBrowser;
                    browser.ObjectForScripting = new Notifier(new Action<string>(
                        (url) =>
                        {
                            if ((context != null) && context.AsCsv)
                            {
                                var csv = url;
                                //var filename = Guid.NewGuid().ToString("N").Substring(0, 8);
                                var saveTo = Path.GetTempFileName();//Path.Combine(Path.GetTempPath(), filename);
                                Util.FileSystem.WriteText(saveTo, "UTF-8", csv);
                                onCompleted(saveTo);
                            }
                            else
                            {
                                //var saveTo = String.Format(@"{0}\{1}.xls", System.Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), url.Substring(url.LastIndexOf('=') + 1));
                                var client = new F.Http.Client();
                                client.Headers[System.Net.HttpRequestHeader.Cookie] = browser.Document.Cookie;
                                using (var st = client.OpenRead(url))
                                {
                                    if ("gzip" != client.ResponseHeaders["Content-Encoding"].ToLower())
                                    {
                                        return;
                                    }
                                    var filename = client.ResponseHeaders["Content-Disposition"].Split(';')[1].Split('=')[1];
                                    var saveTo = Path.Combine(Path.GetTempPath(), filename);
                                    //using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                    //{
                                    //    await fs.WriteAsync(resData, 0, resData.Length);
                                    //    fs.Close();
                                    //}

                                    var decompStream = new System.IO.Compression.GZipStream(st, System.IO.Compression.CompressionMode.Decompress);
                                    var outStream = new FileStream(saveTo, FileMode.Create);
                                    using (outStream)
                                    using (decompStream)
                                    {
                                        while (true)
                                        {
                                            byte[] buf = new byte[1024];
                                            var num = decompStream.Read(buf, 0, buf.Length);
                                            if (0 == num)
                                            {
                                                break;
                                            }
                                            outStream.Write(buf, 0, num);
                                        }
                                    }
                                    onCompleted(saveTo);
                                }
                                //client.DownloadFile(url, saveTo);
                                //var content = client.DownloadString(url);
                            }
                        }));
                    browser.Document.InvokeScript("eval", new object[] { editor.ToString() });
                }));
        }

        private static string JsSalesTransWithCSV(F.ForShop.Work.Journals.Ref.Context context)
        {
            var jsSalesTransWithCSV = "";
            //現在のコードを実行しているAssemblyを取得
            var myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            //指定されたマニフェストリソースを読み込む
            using (var sr = new StreamReader(myAssembly.GetManifestResourceStream("FMWW.Controls.js.SalesTransWithCSV.js"), Encoding.UTF8))
            {
                //内容を読み込む
                jsSalesTransWithCSV = sr.ReadToEnd();
                //後始末
                sr.Close();
                Debug.WriteLine(jsSalesTransWithCSV);
            }
            return jsSalesTransWithCSV;
        }

        // MD分析 -> 店舗売上集計[照会]
        public static Client RefGrossStoreSalesInMdAnalysis(Filter filter, Action<List<string[]>> onCompleted)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 5, SubMenu = 6, SubMenuItem = 1 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() == args.Url.ToString())
                    // /JMODE_ASP/faces/contents/A250_SALES_PRESENT_TTL/A250_REPORT.jsp
                    {
                        var editor = new StringBuilder();

                        editor.AppendLine("(function(){");
                        editor.AppendLine("  function $(id) {");
                        editor.AppendLine("    return document.getElementById(id);");
                        editor.AppendLine("  }");

                        editor.AppendLine("  var contains = function(xs, value) {");
                        editor.AppendLine("      for(var i = 0; i < xs.length; i++){");
                        editor.AppendLine("          if(xs[i].toString() === value.toString()){");
                        editor.AppendLine("              return true;");
                        editor.AppendLine("          }");
                        editor.AppendLine("      }");
                        editor.AppendLine("      return false;");
                        editor.AppendLine("  };");

                        editor.AppendLine("  var setUniversalEntry = function() {");
                        editor.AppendLine("      $('date001').onfocus();");
                        editor.AppendLine("      $('date001').value = '" + filter.From.ToShortDateString() + "';");
                        editor.AppendLine("      $('date001').onblur();");
                        editor.AppendLine("      $('date002').onfocus();");
                        editor.AppendLine("      $('date002').value = '" + filter.To.ToShortDateString() + "';");
                        editor.AppendLine("      $('date002').onblur();");

                        editor.AppendLine("      $('form1:ps').getElementsByTagName('input')[2].click();");
                        editor.AppendLine("      $('form1:sort_cd').getElementsByTagName('input')[1].click();");

                        editor.AppendLine("      $('form1:rank_width').onfocus();");
                        editor.AppendLine("      $('form1:rank_width').value = '30';");
                        editor.AppendLine("      $('form1:rank_width').onblur();");
                        editor.AppendLine("  };");

                        editor.AppendLine("  var pickUpBrand = function(brands) {");
                        editor.AppendLine("    var max = $('brand_cd:SELECT').options.length;");
                        editor.AppendLine("    $('brand_cd:SELECT').onfocus();");
                        editor.AppendLine("    for(var i = 0; i < max; i++) {");
                        editor.AppendLine("      if (contains(brands, $('brand_cd:SELECT').options[i].value)) {");
                        editor.AppendLine("        $('brand_cd:SELECT').options[i].selected = true;");
                        editor.AppendLine("      }");
                        editor.AppendLine("    }");
                        editor.AppendLine("    $('brand_cd:SELECT').onblur();");
                        editor.AppendLine("  };");

                        editor.AppendLine("  var pickUpLine = function(lines) {");
                        editor.AppendLine("    var max = $('line_cd:SELECT').options.length;");
                        editor.AppendLine("    $('line_cd:SELECT').onfocus();");
                        editor.AppendLine("    for (var i = 0; i < max; i++) {");
                        editor.AppendLine("      if (contains(lines, $('line_cd:SELECT').options[i].value)) {");
                        editor.AppendLine("        $('line_cd:SELECT').options[i].selected = true;");
                        editor.AppendLine("      }");
                        editor.AppendLine("    }");
                        editor.AppendLine("    $('line_cd:SELECT').onblur();");
                        editor.AppendLine("  };");

                        editor.AppendLine("  setTimeout(function(){");
                        editor.AppendLine("    if (!$('search_button').click) {");
                        editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        editor.AppendLine("      return;");
                        editor.AppendLine("    }");

                        editor.AppendLine("    setUniversalEntry();");
                        editor.AppendLine("    $('dest_cd:FEELD').onfocus();");
                        editor.AppendLine("    $('dest_cd:FEELD').value = '" + (filter.ShopCode ?? "") + "';");
                        editor.AppendLine("    $('dest_cd:FEELD').onblur();");

                        editor.AppendLine("    $('dest_class:FEELD').onfocus();");
                        editor.AppendLine("    $('dest_class:FEELD').value = '" + (filter.ShopClass ?? "") + "';");
                        editor.AppendLine("    $('dest_class:FEELD').onblur();");

                        editor.AppendLine("    pickUpBrand(['" + String.Join("','", (filter.Brands ?? new HashSet<string>())) + "']);");
                        editor.AppendLine("    pickUpLine(['" + String.Join("','", (filter.Lines ?? new HashSet<string>())) + "']);");

                        editor.AppendLine("    $('search_button').click();");
                        editor.AppendLine("  }, 1000);");
                        editor.AppendLine("})();");

                        //System.Text.Encoding src = System.Text.Encoding.UTF8;
                        //System.Text.Encoding dest = System.Text.Encoding.GetEncoding("Shift_JIS");
                        //byte[] temp = src.GetBytes(script);
                        //byte[] sjis_temp = System.Text.Encoding.Convert(src, dest, temp);
                        //script = dest.GetString(sjis_temp);



                        Debug.WriteLine("店舗売上集計表 条件入力");
                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { editor.ToString() });
                    }
                }))
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/A250_SALES_PRESENT_TTL/A250_SELECT.jsp" }.Uri.ToString() == args.Url.ToString())
                    {
                        Debug.WriteLine("店舗売上集計表 結果");
                        var script = "(function(){"
                        + "  function $(id) {"
                        + "    return document.getElementById(id);"
                        + "  }"
                        + ""
                        + "  var extractRankingFromHtml = function() {"
                        + "      var header = [0,1,4,5];"
                        + "      var products = [];"
                        + "      var table = $('list1:maintables').getElementsByTagName('table')[0];"
                        + "      if(!table) {"
                        + "          return [];"
                        + "      }"
                        + "      for (var i = 1; i < table.rows.length - 1; i++ ) {"
                        + "          var row = table.rows[i];"
                        + "          var record = [];"
                        + "          for (var j = 0; j < header.length; j++ ) {"
                        + "              record.push(row.cells[header[j]].getElementsByTagName('span')[0].innerHTML);"
                        + "          }"
                        + "          record.push($('form1:text1').innerHTML);/* ライン */"
                        + "          record.push($('form1:text9').innerHTML);/* 店舗 */"
                        + "          record.push($('form1:text4').innerHTML);/* 売上期間 */"
                        + "          record.push($('form1:text8').innerHTML);/* 店種 */"
                        + "          products.push(record);"
                        + "      }"
                        + "      return products;"
                        + "  };"
                        + ""
                        + "  setTimeout(function(){"
                        + "    if ('none' != $('list1:MSGDIV').style.display) {"
                        + "      setTimeout(arguments.callee, 1000);\n"
                        + "      return;"
                        + "    }"
                        + ""
                        + "    var json = JSON.stringify(extractRankingFromHtml());"
                        + "    window.external.Completed(json);"
                        + "  }, 1000);"
                        + "})();";

                        var doc = (sender as WebBrowser).Document;
                        (sender as WebBrowser).ObjectForScripting = new Notifier(new Action<string>(
                            (jsonText) =>
                            {
                                Debug.WriteLine(jsonText);
                                var serializer = new DataContractJsonSerializer(typeof(List<string[]>));
                                byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
                                var ms = new MemoryStream(bytes);
                                var ranking = (List<string[]>)serializer.ReadObject(ms);
                                onCompleted(ranking);
                            }));
                        doc.InvokeScript("eval", new object[] { script });
                    }
                }));
        }

        // 店舗一覧
        public static Client RefStores(Action<String> onCompleted)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 1, SubMenu = 6, SubMenuItem = 1 })
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() == args.Url.ToString())
                    {
                        var editor = new StringBuilder();
                        editor.AppendLine("(function(){");
                        editor.AppendLine("  setTimeout(function(){");
                        editor.AppendLine("    if ((!document.getElementById('dest:dest:SELECT')) || (0 == document.getElementById('dest:dest:SELECT').children.length)) {");
                        editor.AppendLine("      setTimeout(arguments.callee, 1000);");
                        editor.AppendLine("      return;");
                        editor.AppendLine("    }");
                        editor.AppendLine("    var stores = [];");
                        editor.AppendLine("    for(var i = 0; i < document.getElementById('dest:dest:SELECT').children.length; ++i) {");
                        editor.AppendLine("      stores.push(document.getElementById('dest:dest:SELECT').children[i].innerHTML.split(' '));");
                        editor.AppendLine("    }");
                        editor.AppendLine("    window.external.Completed(JSON.stringify(stores));");
                        editor.AppendLine("  }, 1000);");
                        editor.AppendLine("})();");
                        var doc = (sender as WebBrowser).Document;
                        doc.InvokeScript("eval", new object[] { editor.ToString() });
                        var browser = sender as NonDispBrowser;
                        browser.ObjectForScripting = new Notifier(new Action<string>(
                            (text) =>
                            {
                                onCompleted(text);
                            }));
                    }
                }));
        }








        public void OnSearchDistributes(object sender, WebBrowserDocumentCompletedEventArgs args)
        {
            if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() == args.Url.ToString() ||
                new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_LIST.jsp" }.Uri.ToString() == args.Url.ToString())
            {
                if (0 == this.DistributeCodes.Count())
                {
                    return;
                }
                var code = this.DistributeCodes.Dequeue();
                var doc = (sender as WebBrowser).Document;
                doc.InvokeScript("eval", new object[] { "setTimeout(function(){"
                        + "if (!document.getElementById('search_button').onclick) {"
                        + "    setTimeout(arguments.callee, 1000);\n"
                        + "    return;"
                        + "}"
                        + "  document.getElementById('status').value=''\n"
                        + "  document.getElementById('distribute_cd_from').value='" + code + "'\n"
                        + "  document.getElementById('distribute_cd_to').value='" + code + "'\n"
                        + "  document.getElementById('search_button').click();"
                        + "}, 1000);" });
                this.Browser.DocumentCompleted -= _documentCompletedEventHandler.Dequeue();
            }
        }

        public void OnDistributesList(object sender, WebBrowserDocumentCompletedEventArgs args)
        {
            if (new UriBuilder(SchemeName, HostName) { Path = "/JMODE_ASP/faces/contents/X018_160_DISTRIBUTE/X018_SELECT.jsp" }.Uri.ToString() == args.Url.ToString())
            {
                (sender as WebBrowser).ObjectForScripting = new Notifier((m) =>
                {
                    Debug.WriteLine(m);
                });
                var doc = (sender as WebBrowser).Document;
                doc.InvokeScript("eval", new object[] { "setTimeout(function(){"
                        + "  if (0 === document.getElementById('list').children.length) {"
                        + "      setTimeout(arguments.callee, 1000);\n"
                        + "      return;"
                        + "  }"
                        + "  var values = [];"
                        + "  var spans = document.getElementById('list').children[1].children[1].getElementsByTagName('tr')[1].getElementsByTagName('span');\n"
                        + "  for (var i = 0; i < spans.length; i++) {\n"
                        + "    values.push(spans[i].innerText);"
                        + "  }"
                        + "  window.external.Completed(values.join());"
                        + "  document.getElementById('quit_button').click();"
                        + "}, 1000);" });
                this.Browser.DocumentCompleted -= _documentCompletedEventHandler.Dequeue();
            }
        }
    }
}
