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
    public class SpecifyFundSellService : MyDisposable
    {
        private const string SellUrlTemplate = "https://danjuanapp.com/position/fund/{fundId}";
        private string _sellUrl;
        private string _amount;
        private const string LoginUrl = "https://danjuanapp.com/account/login";
        private ChromiumWebBrowser _browser;

        private string _result;
        private bool _done;
        private Form _f;

        public SpecifyFundSellService()
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

            _f = new MountForm {Text = @"SellForm"};
            _f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            _browser.RequestHandler = new MyRequestHandler();
            Control.CheckForIllegalCrossThreadCalls = false;

            CombineLog.LogInfo("SpecifyFundSellService class is constructed");
            Name = "SpecifyFundSellService";

        }


        public string ExecuteSell(string fundId, string amount)
        {
            CombineLog.LogInfo("start ExecuteSell");
            _sellUrl = SellUrlTemplate.Replace("{fundId}", fundId);
            _amount = amount;
            if (string.IsNullOrEmpty(_sellUrl))
            {
                throw new Exception("fund id is not set");
            }

            Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    CombineLog.LogInfo("start render form in SpecifyFundSellService");
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
            _browser.Load(LoginUrl);
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

            CombineLog.LogInfo("end ExecuteSell method");
            CombineLog.LogInfo("");
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
                        CombineLog.LogInfo(p.Url);
                    }
                    if (p != null && p.Url == (LoginUrl))
                    {
                        Debug.WriteLine("start to login");
                        FileLog.Info("start to login", LogType.Info);
                        string jscript = "$(\'.pass_switch\').click();$(\'#telno\').val(\'18526088356\');$(\'#pass\').val(\'sundacheng838578\');$(\'#next\').click();";
                        Task t = browser.EvaluateScriptAsync(jscript);
                        Task.WaitAll(new Task[] { t });
                        FileLog.Info("already login, sleep 10 seconds", LogType.Info);
                        Debug.WriteLine("already login, sleep 10 seconds");
                        Thread.Sleep(1000 * 10);
                        FileLog.Info("tring to load sell page", LogType.Info);
                        Debug.WriteLine("tring to load sell page");
                        this._browser.Load(_sellUrl);
                        FileLog.Info("trying to show sell page", LogType.Info);
                        Debug.WriteLine("trying to show sell page");
                    }
                    if (p != null && p.Url == _sellUrl)
                    {
                        FileLog.Info("sell page loaded", LogType.Info);
                        Debug.WriteLine("sell page loaded");

                        Thread.Sleep(1000 * 5);

                        string jscript1 = "$(\'a:contains(\"卖出\")\')[0].click()";
                        Task t1 = browser.EvaluateScriptAsync(jscript1);
                        Task.WaitAll(new Task[] { t1 });
                        CombineLog.LogInfo("sell button clicked sleep 15s");

                        Thread.Sleep(1000 * 5);
                        string jscript2 = "$(\'span:contains(\"现金宝\")\').click()";
                        Task t2 = browser.EvaluateScriptAsync(jscript2);
                        Task.WaitAll(new Task[] { t2 });
                        CombineLog.LogInfo("show channel clicked sleep 15s");
                        Thread.Sleep(1000 * 5);

                        string jscript3 = "$(\"input[name=\'招商银行(6085)\']\").click()";
                        Task t3 = browser.EvaluateScriptAsync(jscript3);
                        Task.WaitAll(new Task[] { t3 });
                        CombineLog.LogInfo("channel is selected sleep 15s");
                        Thread.Sleep(1000 * 5);

                        CombineLog.LogInfo("start input tab");
                        KeyEvent k1 = new KeyEvent
                        {
                            WindowsKeyCode = 0x09,
                            IsSystemKey = false,
                            Type = KeyEventType.KeyDown
                        };
                        browser.GetBrowser().GetHost().SendKeyEvent(k1);
                        KeyEvent k2 = new KeyEvent
                        {
                            WindowsKeyCode = 0x09,
                            IsSystemKey = false,
                            Type = KeyEventType.KeyUp
                        };
                        browser.GetBrowser().GetHost().SendKeyEvent(k2);
                        CombineLog.LogInfo("tab is sent sleep 15s");
                        Thread.Sleep(1000*5);

                        KeyEvent k = new KeyEvent
                        {
                            WindowsKeyCode = Convert.ToInt32(_amount) + 48,
                            FocusOnEditableField = true,
                            IsSystemKey = false,
                            Type = KeyEventType.Char
                        };
                        browser.GetBrowser().GetHost().SendKeyEvent(k);
                        CombineLog.LogInfo("share amount sent sleep 15s");
                        Thread.Sleep(1000*5);

                        string jscript4 = "$(\'button[class=\"dj-button\"]\').click()";
                        Task t4 = browser.EvaluateScriptAsync(jscript4);
                        Task.WaitAll(new Task[] { t4 });
                        CombineLog.LogInfo("subbmited sleep 15s");
                        Thread.Sleep(1000 * 5);

                        string jscript5 = "$(\'input[class=p1]\').val(\'83857\');";
                        Task t5 = browser.EvaluateScriptAsync(jscript5);
                        Task.WaitAll(new Task[] { t5 });
                        Thread.Sleep(1000 * 5);

                        KeyEvent k3 = new KeyEvent
                        {
                            WindowsKeyCode = 0x38,
                            FocusOnEditableField = true,
                            IsSystemKey = false,
                            Type = KeyEventType.Char
                        };
                        browser.GetBrowser().GetHost().SendKeyEvent(k3);
                        CombineLog.LogInfo("password sent sleep 35s");
                        Thread.Sleep(15*1000);

                        CombineLog.LogInfo("start to get result");
                        string j6 = "$(\'h1:contains(\"份\")\').text();";
                        CombineLog.LogInfo("trying to get result info");
                        Task<CefSharp.JavascriptResponse> t6 = browser.EvaluateScriptAsync(j6);
                        Task.WaitAll(new Task[] { t6 });
                        this._result = t6.Result.Result.ToString();
                        _done = true;
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
            _browser?.Dispose();
            _f?.Dispose();
            _browser = null;
            _f = null;
        }
    }

}