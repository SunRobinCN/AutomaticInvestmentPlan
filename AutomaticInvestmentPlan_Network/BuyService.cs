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

        private string _result;
        private bool _done;
        private readonly Form _f;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

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

            _f = new MountForm {Text = @"BuyForm"};
            _f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            _browser.RequestHandler = new MyRequestHandler();
            Control.CheckForIllegalCrossThreadCalls = false;

            FileLog.Info("BuySerice class is constructed", LogType.Info);
            Debug.WriteLine("BuySerice class is constructed");

        }


        public string ExecuteBuy()
        {
            FileLog.Info("start ExecuteBuy method", LogType.Info);
            Debug.WriteLine("start ExecuteBuy method");

            Task.Factory.StartNew2(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    FileLog.Info("start render form in BuyService", LogType.Info);
                    Debug.WriteLine("start render form in BuyService");
                    Application.Run(_f);
                }
            });

            DateTime beginTime = DateTime.Now;
            FileLog.Info("trying to load login page", LogType.Info);
            Debug.WriteLine("trying to load login page");
            bool signal = true;
            while (signal)
            {
                if (this._browser.IsBrowserInitialized == false)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    signal = false;
                }
            }
            _browser.Load(_loginUrl);
            FileLog.Info("login page is loading", LogType.Info);
            Debug.WriteLine("login page is loading");
            Task t =Task.Factory.StartNew2(() =>
            {
                while (_done == false)
                {
                    TimeSpan midTime = DateTime.Now - beginTime;
                    if (midTime.TotalMinutes > 3)
                    {
                        _tokenSource.Cancel();
                        _browser.Dispose();
                        _f.Dispose();
                        throw new Exception("crawl time out");
                    }
                    Thread.Sleep(1000 * 2);
                }
                _tokenSource.Cancel();
                _browser.Dispose();
                _f.Dispose();
            });

            Task.WaitAll(t);
            FileLog.Info("end ExecuteBuy method", LogType.Info);
            FileLog.Info("return result is " + this._result, LogType.Info);
            return _result;
        }

        void OnFrameLoadEnd(object sender, EventArgs e)
        {
            Task.Factory.StartNew2(() =>
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

                        Thread.Sleep(1000 * 5);

                        string jscript1 = $"$(\'input[Name=\"amount\"]\').val({CacheUtil.BuyAmount});";
                        Task t1 = browser.EvaluateScriptAsync(jscript1);
                        Task.WaitAll(new Task[] { t1 });
                        FileLog.Info("amount is already input", LogType.Info);
                        Debug.WriteLine("amount is already input");
                        Thread.Sleep(1000 * 3);

                        string jscript2 = "$(\'button[class=\"dj-button\"]\').removeAttr(\"disabled\");";
                        Task t2 = browser.EvaluateScriptAsync(jscript2);
                        Task.WaitAll(new Task[] { t2 });

                        FileLog.Info("start to submit", LogType.Info);
                        Debug.WriteLine("start to submit");
                        string jscript3 = "$(\'button[class=\"dj-button\"]\').click();";
                        Task t3 = browser.EvaluateScriptAsync(jscript3);
                        Task.WaitAll(new Task[] { t3 });
                        FileLog.Info("form submitted", LogType.Info);
                        Debug.WriteLine("form submitted");
                        Thread.Sleep(1000 * 5);

                        FileLog.Info("start password", LogType.Info);
                        Debug.WriteLine("start password");

                        string jscript4 = "$(\'input[class=p1]\').val(\'83857\');";
                        Task t4 = browser.EvaluateScriptAsync(jscript4);
                        Task.WaitAll(new Task[] { t4 });
                        Thread.Sleep(1000 * 5);

                        FileLog.Info("start key event", LogType.Info);
                        Debug.WriteLine("start key event");
                        KeyEvent k = new KeyEvent();
                        k.WindowsKeyCode = 0x38;
                        k.FocusOnEditableField = true;
                        k.IsSystemKey = false;
                        k.Type = KeyEventType.Char;
                        browser.GetBrowser().GetHost().SendKeyEvent(k);

                        FileLog.Info("password is done", LogType.Info);
                        Debug.WriteLine("password is done");

                        Thread.Sleep(1000 * 15);
                        FileLog.Info("purchase is ok", LogType.Info);
                        Debug.WriteLine("purchase is ok");
                        string j = "$(\"span:contains(\'￥\')\").parent().text();";
                        FileLog.Info("trying to get result info", LogType.Info);
                        Task<CefSharp.JavascriptResponse> t11 = browser.EvaluateScriptAsync(j);
                        Task.WaitAll(new Task[] { t11 });
                        this._result = t11.Result.Result.ToString();
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
        }

        void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Contains("Uncaught") && (e.Message.Contains("modori") == false)
                && (e.Message.Contains("setPostLink") == false))
            {
            }
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs args)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                //browser.ShowDevTools();
            }
        }

    }

}