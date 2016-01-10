// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.Example;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using System.Threading;
using CefSharp.Wpf.Copy.Helpers;
using System.Text.RegularExpressions;

namespace CefSharp.Wpf.Copy.ViewModels
{
    public class BrowserTabViewModel : ViewModelBase
    {
        private string address;
        public string Address
        {
            get { return address; }
            set { Set(ref address, value); }
        }

        private string addressEditable;
        public string AddressEditable
        {
            get { return addressEditable; }
            set { Set(ref addressEditable, value); }
        }

        private string outputMessage;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { Set(ref outputMessage, value); }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { Set(ref statusMessage, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        private IWpfWebBrowser webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set { Set(ref webBrowser, value); }
        }

        private object evaluateJavaScriptResult;
        public object EvaluateJavaScriptResult
        {
            get { return evaluateJavaScriptResult; }
            set { Set(ref evaluateJavaScriptResult, value); }
        }

        private object statusMsg;
        public object StatusMsg
        {
            get { return statusMsg; }
            set { Set(ref statusMsg, value); }
        }

        private bool showSidebar;
        public bool ShowSidebar
        {
            get { return showSidebar; }
            set { Set(ref showSidebar, value); }
        }

        private bool isGetList = false;

        public ICommand GoCommand { get; private set; }
        public ICommand HomeCommand { get; private set; }
        public ICommand ExecuteJavaScriptCommand { get; private set; }
        public ICommand EvaluateJavaScriptCommand { get; private set; }
        public ICommand ShowDevToolsCommand { get; private set; }
        public ICommand CloseDevToolsCommand { get; private set; }
        public ICommand RunCommand { get; private set; }

        public BrowserTabViewModel(string address)
        {
            Address = address;
            AddressEditable = Address;

            GoCommand = new RelayCommand(Go, () => !String.IsNullOrWhiteSpace(Address));
            HomeCommand = new RelayCommand(() => AddressEditable = Address = CefExample.DefaultUrl);
            ExecuteJavaScriptCommand = new RelayCommand<string>(ExecuteJavaScript, s => !String.IsNullOrWhiteSpace(s));
            EvaluateJavaScriptCommand = new RelayCommand<string>(EvaluateJavaScript, s => !String.IsNullOrWhiteSpace(s));
            ShowDevToolsCommand = new RelayCommand(() => webBrowser.ShowDevTools());
            CloseDevToolsCommand = new RelayCommand(() => webBrowser.CloseDevTools());
            //RunCommand = new RelayCommand<ListBox>(Run, OrderList => OrderList.IsInitialized);

            PropertyChanged += OnPropertyChanged;

            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            OutputMessage = version;
        }

        private async void GetInnerHTML()
        {
            try
            {
                var response = await webBrowser.EvaluateScriptAsync("myBytes=window.document.body.innerHTML;");
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("This is a callback from EvaluateJavaScript");
                }

                MainWindow.strInnerHtml = (string)(response.Success ? (response.Result ?? "null") : response.Message);
                ParseHtmlHelper m_ParseHtmlHelper = new ParseHtmlHelper((string)MainWindow.strInnerHtml);
                m_ParseHtmlHelper.GetMessageUrl();

                for (int i = 0; i < m_ParseHtmlHelper.MessageUrlMatchs.Count; i++)
                {
                    string m_Message = m_ParseHtmlHelper.MessageUrlMatchs[i].Groups[1].Captures[0].Value;  /* need handle */
                    string ifrm_name = string.Format(@"frm{0}", i);
                    string JsCmd = "document.body.insertAdjacentHTML(\"beforeEnd\", \"<iframe width='400' height='50' id='" + ifrm_name + "' src='" + m_Message + "'/>\");";
                    response = await  webBrowser.EvaluateScriptAsync(JsCmd);
                    Random ran = new Random(); /* 搞个随机数防检测 */
                    int RandKey = ran.Next(200, 1500);
                    Thread.Sleep(RandKey);
                    JsCmd = "var iFrame = document.getElementById('" + ifrm_name + "');var iFrameElement = iFrame.contentWindow.document.body.innerHTML;MyByte=iFrameElement;";
                    response = await webBrowser.EvaluateScriptAsync(JsCmd);
                    MainWindow.strMessageHtml += (string)(response.Success ? (response.Result ?? "null") : response.Message);
                }

                response = await webBrowser.EvaluateScriptAsync("myBytes=window.document.body.innerHTML;");
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("This is a callback from EvaluateJavaScript");
                }

                MainWindow.strInnerHtml = (string)(response.Success ? (response.Result ?? "null") : response.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while evaluating Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EvaluateJavaScript(string s)
        {
            try
            {
                var response = await webBrowser.EvaluateScriptAsync(s);
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("This is a callback from EvaluateJavaScript");
                }

                EvaluateJavaScriptResult = response.Success ? (response.Result ?? "null") : response.Message;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while evaluating Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteJavaScript(string s)
        {
            try
            {
                webBrowser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    AddressEditable = Address;
                    break;

                case "WebBrowser":
                    if (WebBrowser != null)
                    {
                        WebBrowser.ConsoleMessage += OnWebBrowserConsoleMessage;
                        WebBrowser.StatusMessage += OnWebBrowserStatusMessage;
                        WebBrowser.LoadError += OnWebBrowserLoadError;

                        // TODO: This is a bit of a hack. It would be nicer/cleaner to give the webBrowser focus in the Go()
                        // TODO: method, but it seems like "something" gets messed up (= doesn't work correctly) if we give it
                        // TODO: focus "too early" in the loading process...
                        WebBrowser.FrameLoadEnd += delegate { Application.Current.Dispatcher.BeginInvoke((Action)(() => webBrowser.Focus())); };
                        WebBrowser.FrameLoadEnd += OnWebBrowserFrameLoadEnd;
                        WebBrowser.LoadingStateChanged += OnWebBrowserLoadingStateChanged;
                    }

                    break;
            }
        }

        private void OnWebBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            OutputMessage = e.Message;
            //EvaluateJavaScriptResult += OutputMessage;
        }

        private void OnWebBrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //this.m_WebView.Browser.GetMainFrame().ExecuteJavaScript("var us=document.getElementById(\"TPL_username_1\");us.value=\"淡定亚强\";alert(us.value);var pw=document.getElementById(\"TPL_password_1\");pw.value=\"l!1y@2q*3\";alert(pw.value);var su=document.getElementById(\"J_SubmitStatic\");su.click();", "", 1);
            //WebBrowser.ExecuteScriptAsync("console.log('just test.')");
        }

        private void OnWebBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.CanReload && (Title.CompareTo("淘宝网 - 淘！我喜欢") == 0) && Address.Contains("www.taobao.com"))
            {
                if (Title.CompareTo("已卖出的宝贝") != 0)
                {
                    Thread.Sleep(1000);
                    WebBrowser.Load("https://trade.taobao.com/trade/itemlist/list_sold_items.htm");
                    StatusMsg += "Load 已卖出的宝贝";
                    isGetList = false;
                }                
            }

            if (e.CanReload && (Title.CompareTo("已卖出的宝贝") == 0) && !isGetList)
            {
                StatusMsg += "get inneHTML";
                isGetList = true;
                GetInnerHTML();
//                string input = (string)evaluateJavaScriptResult;
            }
        }

        private void OnWebBrowserStatusMessage(object sender, StatusMessageEventArgs e)
        {
            StatusMessage = e.Value;
            //EvaluateJavaScriptResult += StatusMessage;
        }

        private void OnWebBrowserLoadError(object sender, LoadErrorEventArgs args)
        {
            // Don't display an error for downloaded files where the user aborted the download.
            if (args.ErrorCode == CefErrorCode.Aborted)
                return;

            var errorMessage = "<html><body><h2>Failed to load URL " + args.FailedUrl +
                  " with error " + args.ErrorText + " (" + args.ErrorCode +
                  ").</h2></body></html>";

            webBrowser.LoadHtml(errorMessage, args.FailedUrl);
        }

        private void Go()
        {
            Address = AddressEditable;

            // Part of the Focus hack further described in the OnPropertyChanged() method...
            Keyboard.ClearFocus();
        }

        public void LoadCustomRequestExample()
        {
            var frame = WebBrowser.GetMainFrame();

            //Create a new request knowing we'd like to use PostData
            var request = frame.CreateRequest(initializePostData:true);
            request.Method = "POST";
            request.Url = "custom://cefsharp/PostDataTest.html";
            request.PostData.AddData("test=123&data=456");

            frame.LoadRequest(request);
        }
    }
}
