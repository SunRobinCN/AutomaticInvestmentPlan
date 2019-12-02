using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomaticInvestmentPlan_Comm;
using CefSharp;
using CefSharp.WinForms;

namespace AutomaticInvestmentPlan_Network
{
    public class GeneralPointService
    {
        private const string Url = "https://www.txfund.com";
        private readonly ChromiumWebBrowser _browser;
        private string _result;
        private bool _done;
        private Form _f;

        public GeneralPointService()
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
            _f = new MountForm();
            _f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            Control.CheckForIllegalCrossThreadCalls = false;

            FileLog.Info("GeneralPointService class is constructed", LogType.Info);
            Debug.WriteLine("GeneralPointService class is constructed");
        }

        public string ExecuteCrawl()
        {
            Task.Factory.StartNew2(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    FileLog.Info("start render form in GeneralPointService", LogType.Info);
                    Debug.WriteLine("start render form in GeneralPointService");
                    Application.Run(_f);
                }
            });

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
            DateTime beginTime = DateTime.Now;
            _browser.Load(Url);
            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > 3)
                {
                    _browser.Dispose();
                    _f.Dispose();
                    throw new Exception("GeneralPointService crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }
            _browser.Dispose();
            _f.Dispose();

            Thread.Sleep(1000*45);
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
                    if (p != null && p.Url == "https://www.txfund.com/")
                    {
                        List<Task> tasks = new List<Task>();
                        string jscript1 = "$(\'.js-s_sh000001\').find(\'.js-num\').html() + \':\' +$(\'.js-s_sh000001\').find(\'.js-rate\').html();";
                        Task<CefSharp.JavascriptResponse> task1 = browser.EvaluateScriptAsync(jscript1);
                        tasks.Add(task1);
                        Task.WaitAll(tasks.ToArray());

                        _result = task1.Result.Result.ToString();
                        _done = true;
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

        private void OnIsBrowserInitializedChanged(object sender, EventArgs args)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                //browser.ShowDevTools();
                //browser.Load(_loginUrl);
            }
        }
    }

    public class JsDialogHandler : IJsDialogHandler
    {
        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, string acceptLang,
            CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback,
            ref bool suppressMessage)
        {
            callback.Continue(true);
            return true;
        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload,
            IJsDialogCallback callback)
        {
            return true;
        }

        public bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType,
            string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            callback.Continue(true);
            return true;
        }

        public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload,
            IJsDialogCallback callback)
        {
            return true;
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
        }
    }

}
