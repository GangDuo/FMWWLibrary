using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FMWW.Controls.FMWW.Master.Product.Alteration
{
    public class Client
    {
        public class Meta
        {
            public string SchemeName { get; set; }
            public string HostName { get; set; }
        }

        private static void Eval(WebBrowser browser, string expression)
        {
            browser.Document.InvokeScript("eval", new object[] { expression });
        }

        private static string BuildScriptToSearchProductBy(string modelNbr)
        {
            return @"
(function(){
  setTimeout(function(){
    if (!document.getElementById('search_button').click) {
      setTimeout(arguments.callee, 1000);
      return;
    }
    var modelNbr = '" + modelNbr + @"';
    document.getElementById('form1:style_cd').value = modelNbr;
    document.getElementById('form1:style_cd2').value = modelNbr;
    document.getElementById('search_button').click();
  }, 1000);
})();";
        }

        private static string BuildScriptToClickFirstChild(string key)
        {
            return @"
(function(){
  setTimeout(function(){
    if ((!document.getElementById('SlipList:COL_0')) && (0 === document.getElementById('SlipList:LIMIT_CAUTION').innerHTML.length)) {
      setTimeout(arguments.callee, 1000);
      return;
    }
    if(document.getElementById('SlipList:COL_0')) {
      document.getElementById('SlipList:COL_0').firstChild.click();
      return;
    }
    var key = '" + key + @"';
    window.external.Completed(key + ':表示するデータがありません。\\n');
  }, 1000);
})();
window.alert = function(){};";
        }

        private static string BuildScriptToRemoveProduct()
        {
            return @"
(function() {
  document.getElementById('form1:delete').click();
})();";
        }

        private static string BuildScriptToClose(string key)
        {
            return @"
(function() {
  var message = document.getElementById('form1:message').innerHTML || '';
  window.external.Completed(message);
  window.external.Terminated('" + key + @"');
})();";
        }

        public static Controls.Client DeleteByModelNbr(Meta me, string modelNbr, Action<string> completed)
        {
            return new Controls.Client(new Controls.MainMenu() { Menu = 9, SubMenu = 12, SubMenuItem = 3 })
            #region 商品検索画面
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(me.SchemeName, me.HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }
                    Eval((sender as WebBrowser), BuildScriptToSearchProductBy(modelNbr));
                }))
            #endregion
            #region 商品一覧画面
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(me.SchemeName, me.HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }
                    var browser = sender as NonDispBrowser;
                    browser.ObjectForScripting = new Notifier(new Action<string>(
                        (text) =>
                        {
                            Debug.WriteLine(text);
                            completed(text);
                        }));
                    Eval(browser, BuildScriptToClickFirstChild(modelNbr));
                }))
            #endregion
            #region 商品編集画面
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    if (new UriBuilder(me.SchemeName, me.HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
                    {
                        return;
                    }
                    var browser = sender as NonDispBrowser;
                    browser.ObjectForScripting = new Notifier(new Action<string>(
                        (text) =>
                        {
                            Debug.WriteLine(text);
                            completed(text);
                        }))
                    {
                        OnTerminated = new Action<string>(v =>
                        {
                            Debug.WriteLine("OnTerminated");
                            //if (null == terminated)
                            //{
                            //    return;
                            //}
                            //terminated(v);
                        })
                    };
                    Eval(browser, BuildScriptToRemoveProduct());
                }))
            #endregion
            #region 商品一覧
                .ContinueWith(new WebBrowserDocumentCompletedEventHandler((sender, args) =>
                {
                    // 
                    var doc = (sender as WebBrowser).Document;
                    Debug.WriteLine(doc.Body.ToString());
                    Eval((sender as WebBrowser), BuildScriptToClose(modelNbr));
                }));
            #endregion
        }

        #region key:formのid、value:関連情報
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
        
        public static Controls.Client Edit(System.Data.DataRow product, Meta meta, Action<string> completed, Action<string> terminated)
        {
            string sku = product["SKU"].ToString();
            var vv = new Dictionary<string, string>();
            foreach (var key in IdInfo.Keys)
            {
                try
                {
                    var fieldName = IdInfo[key].Caption;
                    vv.Add(key, product[fieldName].ToString());
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
                        var digit = int.Parse(vv[key], System.Globalization.NumberStyles.AllowThousands);
                        editor.AppendLine(String.Format("if(document.getElementById('{0}').native !== '{1}')", key, digit));
                        editor.AppendLine(String.Format("  document.getElementById('{0}').native = '{1}';{2}", key, digit, Environment.NewLine));
                        break;
                    case EditableItem.Type.CheckBox:
                        var format = "document.getElementById('{0}').checked = {1};";
                        switch (vv[key])
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
                    if (new UriBuilder(meta.SchemeName, meta.HostName) { Path = "/JMODE_ASP/faces/contents/index.jsp" }.Uri.ToString() != args.Url.ToString())
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
                    if (new UriBuilder(meta.SchemeName, meta.HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_SELECT.jsp" }.Uri.ToString() != args.Url.ToString())
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
                    if (new UriBuilder(meta.SchemeName, meta.HostName) { Path = "/JMODE_ASP/faces/contents/M130_PRODUCT/M130_LIST.jsp" }.Uri.ToString() != args.Url.ToString())
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
//                  if ("/JMODE_ASP/faces/contents/M130_PRODUCT/M130_MODIFY.jsp" != args.Url.ToString())
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
    }
}
