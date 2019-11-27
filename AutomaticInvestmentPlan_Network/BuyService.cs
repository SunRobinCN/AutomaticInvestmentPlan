using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomaticInvestmentPlan_Comm;
using CefSharp;
using CefSharp.WinForms;

namespace AutomaticInvestmentPlan_Network
{
    public class BuyService
    {
        private const string _buyUrl = "https://danjuanapp.com/fund/240014/purchase";
        private const string _loginUrl = "https://danjuanapp.com/account/login";
        private readonly ChromiumWebBrowser _browser;

        private int _amount;
        private string _result;
        private bool _done;

        public BuyService()
        {
            if (Cef.IsInitialized == false)
            {
                var settings = new CefSettings();
                CefSharp.Cef.Initialize(settings);
            }
            _browser = new ChromiumWebBrowser("")
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
            };
            Form f = new MountForm();
            f.Text = "BuyForm";
            f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            _browser.RequestHandler = new MyRequestHandler();
            Control.CheckForIllegalCrossThreadCalls = false;
            Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                Application.Run(f);
            });
            FileLog.Info("BuySerice class is constructed", LogType.Info);
            Debug.WriteLine("BuySerice class is constructed");

        }


        public string ExecuteBuy(int amount)
        {
            Thread.Sleep(1000*15);
            FileLog.Info("start ExecuteBuy method", LogType.Info);
            Debug.WriteLine("start ExecuteBuy method");
            this._amount = amount;
            DateTime beginTime = DateTime.Now;
            FileLog.Info("trying to load login page", LogType.Info);
            Debug.WriteLine("trying to load login page");
            _browser.Load(_loginUrl);
            FileLog.Info("login page is loading", LogType.Info);
            Debug.WriteLine("login page is loading");
            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > 3)
                {
                    throw new Exception("crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }
            _browser.Dispose();
            FileLog.Info("end ExecuteBuy method", LogType.Info);
            FileLog.Info("return result is " + this._result, LogType.Info);
            return _result;
        }

        void OnFrameLoadEnd(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
                    CefSharp.FrameLoadEndEventArgs p = e as CefSharp.FrameLoadEndEventArgs;
                    if (p != null)
                    {
                        FileLog.Info(p.Url, LogType.Info);
                        Debug.WriteLine(p.Url);
                    }
                    if (p != null && p.Url == (_loginUrl))
                    {
                        Debug.WriteLine("start to login");
                        FileLog.Info("start to login", LogType.Info);
                        string jscript = "$(\'.pass_switch\').click();$(\'#telno\').val(\'18526088356\');$(\'#pass\').val(\'sundacheng838578\');$(\'#next\').click();";
                        Task t = browser.EvaluateScriptAsync(jscript);
                        Task.WaitAll(new Task[] { t });
                        FileLog.Info("already login, sleep 10 seconds", LogType.Info);
                        Debug.WriteLine("already login, sleep 10 seconds");
                        Thread.Sleep(1000 * 10);
                        FileLog.Info("tring to load pruchase page", LogType.Info);
                        Debug.WriteLine("tring to load pruchase page");
                        this._browser.Load(_buyUrl);
                        FileLog.Info("trying to show purchage page", LogType.Info);
                        Debug.WriteLine("trying to show purchage page");
                    }
                    if (p != null && p.Url == _buyUrl)
                    {
                        FileLog.Info("purchage page loaded", LogType.Info);
                        Debug.WriteLine("purchage page loaded");
                        FileLog.Info("start to execute tabs", LogType.Info);
                        Debug.WriteLine("start to execute tabs");
                        Thread.Sleep(1000 * 8);
                        FileLog.Info("executing tabs", LogType.Info);
                        Debug.WriteLine("executing tabs");
                        this._browser.Focus();
                        SendKeys.SendWait("{TAB}");
                        FileLog.Info("one tab ok", LogType.Info);
                        Debug.WriteLine("one tab ok");
                        Thread.Sleep(1000 * 1);
                        this._browser.Focus();
                        SendKeys.SendWait("{TAB}");
                        FileLog.Info("two tab ok", LogType.Info);
                        Debug.WriteLine("two tab ok");
                        Thread.Sleep(1000 * 1);
                        this._browser.Focus();
                        SendKeys.SendWait("{TAB}");
                        FileLog.Info("three tab ok", LogType.Info);
                        Debug.WriteLine("three tab ok");
                        Thread.Sleep(1000 * 1);
                        this._browser.Focus();
                        SendKeys.SendWait("{TAB}");
                        FileLog.Info("four tab ok", LogType.Info);
                        Debug.WriteLine("four tab ok");
                        Thread.Sleep(1000 * 5);
                        FileLog.Info("start to input amount", LogType.Info);
                        Debug.WriteLine("start to input amount");

                        List<string> amountStrArray = GetStringArray(this._amount.ToString());
                        foreach (string s in amountStrArray)
                        {
                            FileLog.Info("tryint to input key " + s, LogType.Info);
                            Debug.WriteLine("tryint to input key " + s);
                            this._browser.Focus();
                            SendKeys.SendWait(s);
                            Thread.Sleep(1000 * 2);
                        }
                        FileLog.Info("amount is already input", LogType.Info);
                        Debug.WriteLine("amount is already input");
                        Thread.Sleep(1000 * 5);

                        FileLog.Info("start to submit", LogType.Info);
                        Debug.WriteLine("start to submit");
                        string jscript3 = "$(\'button[class=\"dj-button\"]\').click();";
                        Task t = browser.EvaluateScriptAsync(jscript3);
                        Task.WaitAll(new Task[] { t });
                        FileLog.Info("form submitted", LogType.Info);
                        Debug.WriteLine("form submitted");

                        FileLog.Info("start password", LogType.Info);
                        Debug.WriteLine("start password");
                        Thread.Sleep(1000 * 5);
                        this._browser.Focus();
                        Thread.Sleep(1000 * 5);
                        SendKeys.SendWait("8");
                        Thread.Sleep(1000);
                        this._browser.Focus();
                        SendKeys.SendWait("3");
                        Thread.Sleep(1000);
                        this._browser.Focus();
                        SendKeys.SendWait("8");
                        Thread.Sleep(1000);
                        this._browser.Focus();
                        SendKeys.SendWait("5");
                        Thread.Sleep(1000);
                        this._browser.Focus();
                        SendKeys.SendWait("7");
                        Thread.Sleep(1000);
                        this._browser.Focus();
                        SendKeys.SendWait("8");
                        FileLog.Info("password is done", LogType.Info);
                        Debug.WriteLine("password is done");

                        Thread.Sleep(1000*15);
                        FileLog.Info("purchase is ok", LogType.Info);
                        Debug.WriteLine("purchase is ok");
                        string j = "$(\"span:contains(\'￥\')\").parent().text();";
                        FileLog.Info("trying to get result info", LogType.Info);
                        Task<CefSharp.JavascriptResponse> t1 = browser.EvaluateScriptAsync(j);
                        Task.WaitAll(new Task[] { t1 });
                        this._result = t1.Result.Result.ToString();
                        this._done = true;
                        Debug.WriteLine("this operation is done with result " + this._result);
                        FileLog.Info("this operation is done with result " + this._result, LogType.Info);
                    }
                }
                catch (Exception exception)
                {
                    FileLog.Error("OnFrameLoadEnd", exception, LogType.Error);
                }

            });
        }


        void OnLoadError(object sender, LoadErrorEventArgs e)
        {
            //FileLog.Error("OnLoadError", new Exception(e.ErrorText), LogType.Error);
        }

        void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Contains("Uncaught") && (e.Message.Contains("modori") == false)
                && (e.Message.Contains("setPostLink") == false))
            {
                //FileLog.Error("OnConsoleMessage", new Exception(e.Message + "\r\n" + e.Source), LogType.Error);
                //ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
                //browser?.Dispose();
            }
        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                browser.ShowDevTools();
            }
        }

        private List<string> GetStringArray(string s)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                string t = s.Substring(i, 1);
                list.Add(t);
            }
            return list;
        }
    }

}