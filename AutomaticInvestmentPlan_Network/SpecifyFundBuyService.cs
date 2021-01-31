using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Model;
using AutomaticInvestmentPlan_Network.WebHandler;
using CefSharp;
using CefSharp.WinForms;

namespace AutomaticInvestmentPlan_Network
{
    public class SpecifyFundBuyService : MyDisposable
    {
        private const string _buyUrlTemplate = "https://danjuanapp.com/fund/{fundId}/purchase";
        private string _buyUrl;
        private string _amount;
        private const string _loginUrl = "https://danjuanapp.com/account/login";
        private ChromiumWebBrowser _browser;

        private string _result;
        private bool _done;
        private Form _f;

        public SpecifyFundBuyService()
        {
            if (Cef.IsInitialized == false)
            {
                var settings = new CefSettings();
                Cef.Initialize(settings);
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

            CombineLog.LogInfo("BuySerice class is constructed");
            Name = "BuySerice";
        }


        public string ExecuteBuy(string fundId, string amount)
        {
            CombineLog.LogInfo("start ExecuteBuy method");
            _buyUrl = _buyUrlTemplate.Replace("{fundId}", fundId);
            _amount = amount;
            if (string.IsNullOrEmpty(_buyUrl))
            {
                throw new Exception("fund id is not set");
            }

            Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    CombineLog.LogInfo("start render form in SpecifyFundBuyService");
                    Application.Run(_f);
                }
            });

            DateTime beginTime = DateTime.Now;
            CombineLog.LogInfo("trying to load login page");
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
            CombineLog.LogInfo("login page is loading");

            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > Constant.TimeOutMinutes)
                {
                    _browser.Dispose();
                    _f.Dispose();
                    throw new Exception("crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }

            CombineLog.LogInfo("end ExecuteBuy method");
            CombineLog.LogInfo("return result is " + this._result);
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
                        CombineLog.LogInfo("already login, sleep 10 seconds");
                        Thread.Sleep(1000 * 10);
                        CombineLog.LogInfo("tring to load pruchase page");
                        this._browser.Load(_buyUrl);
                        CombineLog.LogInfo("trying to show purchage page");
                    }
                    if (p != null && p.Url == _buyUrl)
                    {
                        CombineLog.LogInfo("purchage page loaded");
                        Thread.Sleep(1000 * 5);

                        string jscript0 = "$(\'.change-icon\').click();";
                        Task t0 = browser.EvaluateScriptAsync(jscript0);
                        Task.WaitAll(new Task[] { t0 });
                        CombineLog.LogInfo("channel is selected");
                        Thread.Sleep(1000 * 3);

                        string jscript00 = "$(\'input[name=\"招商银行(6085)\"]\').click();";
                        Task t00 = browser.EvaluateScriptAsync(jscript00);
                        Task.WaitAll(new Task[] { t00 });
                        CombineLog.LogInfo("channel is clicked");
                        Thread.Sleep(1000 * 3);

                        string jscript1 = $"$(\'input[Name=\"amount\"]\').val({_amount});";
                        Task t1 = browser.EvaluateScriptAsync(jscript1);
                        Task.WaitAll(new Task[] { t1 });
                        CombineLog.LogInfo("amount is already input");
                        Thread.Sleep(1000 * 3);

                        string jscript2 = "$(\'button[class=\"dj-button\"]\').removeAttr(\"disabled\");";
                        Task t2 = browser.EvaluateScriptAsync(jscript2);
                        Task.WaitAll(new Task[] { t2 });

                        CombineLog.LogInfo("start to submit");
                        string jscript3 = "$(\'button[class=\"dj-button\"]\').click();";
                        Task t3 = browser.EvaluateScriptAsync(jscript3);
                        Task.WaitAll(new Task[] { t3 });
                        CombineLog.LogInfo("form submitted");
                        Thread.Sleep(1000 * 5);

                        CombineLog.LogInfo("start password");
                        string jscript4 = "$(\'input[class=p1]\').val(\'83857\');";
                        Task t4 = browser.EvaluateScriptAsync(jscript4);
                        Task.WaitAll(t4);
                        Thread.Sleep(1000 * 5);

                        CombineLog.LogInfo("start key event");
                        KeyEvent k = new KeyEvent();
                        k.WindowsKeyCode = 0x38;
                        k.FocusOnEditableField = true;
                        k.IsSystemKey = false;
                        k.Type = KeyEventType.Char;
                        browser.GetBrowser().GetHost().SendKeyEvent(k);

                        FileLog.Info("password is done", LogType.Info);
                        Debug.WriteLine("password is done");

                        Thread.Sleep(1000 * 15);
                        CombineLog.LogInfo("purchase is ok");
                        string j = "$.trim($(\"h1:contains(\'元\')\").text());";
                        CombineLog.LogInfo("trying to get result info");
                        Task<CefSharp.JavascriptResponse> t11 = browser.EvaluateScriptAsync(j);
                        Task.WaitAll(new Task[] { t11 });
                        this._result = t11.Result.Result.ToString();
                        this._done = true;
                        JobDone = true;
                        CombineLog.LogInfo("this operation is done with result " + this._result);
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
            FileLog.Warn("SpecifyFundBuyService.OnLoadError", new Exception(e.ErrorText), LogType.Warn);
        }

        void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            FileLog.Warn("SpecifyFundBuyService.OnConsoleMessage", new Exception(e.Message + "\r\n" + e.Source), LogType.Warn);
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs args)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                //browser.ShowDevTools();
            }
        }

        public override void Dispose()
        {
            CombineLog.LogInfo("Dispose SpecifyFundBuyService form");
            if (_browser != null && _browser.IsDisposed == false)
            {
                CustomDisposeUtil.Dispose(_browser);
            }
            if (_f != null && _f.IsDisposed == false)
            {
                CustomDisposeUtil.Dispose(_f);
            }
            _browser = null;
            _f = null;
        }
    }

}