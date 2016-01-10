// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Example;
using CefSharp.Wpf.Copy.Handlers;
using CefSharp.Wpf.Copy.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;

namespace CefSharp.Wpf.Copy.Views
{
    public partial class BrowserTabView : UserControl
    {
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<OrderInfo> orderInfos = new ObservableCollection<OrderInfo>();
        int currentPageIndex = 0;
        int itemPerPage = 20;
        int totalPage = 0;

        private string innerHTML;
        public string InnerHTML;

        public BrowserTabView()
        {
            InitializeComponent();

            browser.RequestHandler = new RequestHandler();
            browser.RegisterJsObject("bound", new BoundObject());
            browser.RegisterAsyncJsObject("boundAsync", new AsyncBoundObject());
            // Enable touch scrolling - once properly tested this will likely become the default
            browser.IsManipulationEnabled = true;

            //browser.LifeSpanHandler = new LifespanHandler();
            browser.MenuHandler = new MenuHandler();
            browser.GeolocationHandler = new GeolocationHandler();
            browser.DownloadHandler = new DownloadHandler();
            //You can specify a custom RequestContext to share settings amount groups of ChromiumWebBrowsers
            //Also this is now the only way to access OnBeforePluginLoad - need to implement IPluginHandler
            //browser.RequestContext = new RequestContext(new PluginHandler());
            
            //browser.RequestContext.RegisterSchemeHandlerFactory(CefSharpSchemeHandlerFactory.SchemeName, null, new CefSharpSchemeHandlerFactory());
            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();

            Loaded += BrowserTabView_Loaded;

            btnRun.Click += btnRun_Click;
            
            browser.LoadError += (sender, args) =>
            {
                // Don't display an error for downloaded files.
                if (args.ErrorCode == CefErrorCode.Aborted)
                {
                    return;
                }

                // Don't display an error for external protocols that we allow the OS to
                // handle. See OnProtocolExecution().
                //if (args.ErrorCode == CefErrorCode.UnknownUrlScheme)
                //{
                //	var url = args.Frame.Url;
                //	if (url.StartsWith("spotify:"))
                //	{
                //		return;
                //	}
                //}

                // Display a load error message.
                var errorBody = string.Format("<html><body bgcolor=\"white\"><h2>Failed to load URL {0} with error {1} ({2}).</h2></body></html>",
                                              args.FailedUrl, args.ErrorText, args.ErrorCode);

                args.Frame.LoadStringForUrl(errorBody, args.FailedUrl);
            };

            CefExample.RegisterTestResources(browser);

            ComboBoxType.SelectionChanged += ComboBoxType_SelectionChanged;
        }

        void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //this.browser.Load("https://trade.taobao.com/trade/itemlist/list_sold_items.htm");

            Thread.Sleep(1500);
            
            TextBoxStatus.Text += @"runing";
            LoadText();

            //this.browser.GetMainFrame().EvaluateScriptAsync("alert('AAAAA');");
        }

        public void LoadText()
        {
            string textFile = @"已卖出宝贝";
            FileStream fs;
            string text;
            if (File.Exists(textFile))
            {
                fs = new FileStream(textFile, FileMode.Open, FileAccess.Read);
                using (fs)
                {
                    StreamReader MyStreamReader = new StreamReader(fs, System.Text.Encoding.UTF8);
                    text = MyStreamReader.ReadToEnd();
                    innerHTML = text;
                }
                fs.Close();
            }

            ParseInnerHTML();
        }

        void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        void BrowserTabView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBoxType.Items.Add("全部");
            ComboBoxType.Items.Add("买家已付款");
            ComboBoxType.Items.Add("等待买家付款");
            ComboBoxType.Items.Add("卖家已发货");
            ComboBoxType.Items.Add("交易成功");
            ComboBoxType.Items.Add("交易关闭");
            ComboBoxType.Items.Add("待付款和待发货订单");
            ComboBoxType.Items.Add("定金已付");
            ComboBoxType.Items.Add("异常订单");
        }

        public void ParseInnerHTML()
        {
            ParseHtmlHelper m_ParseHtmlHelper = new ParseHtmlHelper((string)MainWindow.strInnerHtml);
            m_ParseHtmlHelper.ExecuteAllRegex();
            orderInfos.Clear();
            int j = 0;

            for (int i = 0; i < m_ParseHtmlHelper.RegexMatchs[2].Count; i++)   /* SubOrderID Num */
            {
                string m_TradeID = m_ParseHtmlHelper.RegexMatchs[0][j].Groups[1].Captures[0].Value;
                string m_Time_stamp = m_ParseHtmlHelper.RegexMatchs[1][j].Groups[1].Captures[0].Value;
                string m_Item = m_ParseHtmlHelper.RegexMatchs[3][i].Groups[2].Captures[0].Value;
                m_Item = m_Item.Replace("\r", "");
                string m_Price = m_ParseHtmlHelper.RegexMatchs[4][i].Groups[1].Captures[0].Value;
                string m_Num = m_ParseHtmlHelper.RegexMatchs[5][i].Groups[1].Captures[0].Value;
                string m_Trouble = m_ParseHtmlHelper.RegexMatchs[6][i].Groups[0].Captures[0].Value;
                string m_Contact = m_ParseHtmlHelper.RegexMatchs[7][i].Groups[1].Captures[0].Value;
                string m_Trade_status = m_ParseHtmlHelper.RegexMatchs[8][i].Groups[2].Captures[0].Value;  /* need handle */
                m_Trade_status = m_Trade_status.Replace("\r", "");
                string m_Order_price = m_ParseHtmlHelper.RegexMatchs[9][i].Groups[1].Captures[0].Value;
                string m_Remark = m_ParseHtmlHelper.RegexMatchs[10][i].Groups[0].Captures[0].Value;
                string m_Message = m_ParseHtmlHelper.RegexMatchs[11][i].Groups[1].Captures[0].Value;

                if (m_Trade_status.Contains("</strong>"))
                    m_Trade_status = m_Trade_status.Replace("</strong>", "");

                orderInfos.Add(new OrderInfo()
                {
                    TradeID = m_ParseHtmlHelper.RegexMatchs[0][j].Groups[1].Captures[0].Value,
                    Time_stamp = m_ParseHtmlHelper.RegexMatchs[1][j].Groups[1].Captures[0].Value,
                    Item = m_Item,
                    Price = m_ParseHtmlHelper.RegexMatchs[4][i].Groups[1].Captures[0].Value,
                    Num = m_ParseHtmlHelper.RegexMatchs[5][i].Groups[1].Captures[0].Value,
                    Trouble = m_ParseHtmlHelper.RegexMatchs[6][i].Groups[0].Captures[0].Value,
                    Contact = m_ParseHtmlHelper.RegexMatchs[7][i].Groups[1].Captures[0].Value,
                    Trade_status = m_Trade_status,
                    Order_price = m_ParseHtmlHelper.RegexMatchs[9][i].Groups[1].Captures[0].Value,
                    Remark = m_ParseHtmlHelper.RegexMatchs[10][i].Groups[0].Captures[0].Value,
                    Message = m_Message
               });

                if (m_ParseHtmlHelper.RegexMatchs[0][j].Groups[1].Captures[0].Value.CompareTo(
                        m_ParseHtmlHelper.RegexMatchs[2][i].Groups[1].Captures[0].Value) != 0)
                    j++;
            }

            view.Source = orderInfos;

            this.OrderList.DataContext = view;
        }

        private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void OnTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
