using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    public class GeneralPointService : MyDisposable
    {
        private const string Url = "https://www.txfund.com";
        private ChromiumWebBrowser _browser;
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

            CombineLog.LogInfo("GeneralPointService class is constructed");
            Name = "GeneralPointService";
        }

        public string ExecuteCrawl()
        {
            Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    CombineLog.LogInfo("start render form in GeneralPointService");
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
                        Thread.Sleep(1000 * 2);
                        List<Task> tasks = new List<Task>();
                        string jscript1 = "$(\'.js-s_sh000001\').find(\'.js-num\').html() + \':\' +$(\'.js-s_sh000001\').find(\'.js-rate\').html();";
                        Task<CefSharp.JavascriptResponse> task1 = browser.EvaluateScriptAsync(jscript1);
                        tasks.Add(task1);
                        Task.WaitAll(tasks.ToArray());

                        _result = task1.Result.Result.ToString();
                        _done = true;
                        JobDone = true;
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
            FileLog.Error("GeneralPointService.OnLoadError", new Exception(e.ErrorText), LogType.Error);
        }

        void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            FileLog.Error("GeneralPointService.OnConsoleMessage", new Exception(e.Message + "\r\n" + e.Source), LogType.Error);
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs args)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                //browser.ShowDevTools();
                //browser.Load(_loginUrl);
            }
        }

        public override void Dispose()
        {
            CombineLog.LogInfo("Dispose GeneralPointService form");
            if (_browser != null && _browser.IsDisposed == false)
            {
                _browser?.Dispose();
            }
            if (_f != null && _f.IsDisposed == false)
            {
                _f?.Dispose();
            }
            _browser = null;
            _f = null;
        }
    }

    

}
