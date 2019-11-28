using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomaticInvestmentPlan_Comm;
using CefSharp;
using CefSharp.WinForms;

namespace AutomaticInvestmentPlan_Network
{
    public class SpecifyFundHistoryJumpService
    {
        private string _url = "https://danjuanapp.com/net-history/";
        private readonly ChromiumWebBrowser _browser;
        private string _result;
        private bool _done;
        private Task _t;
        CancellationTokenSource _cts = new CancellationTokenSource();
        private CancellationToken _ct;

        public SpecifyFundHistoryJumpService()
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
            f.Visible = false;
            f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            Control.CheckForIllegalCrossThreadCalls = false;
            _t = Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                Application.Run(f);
            }, _ct);
            //f.Show();
        }

        public string ExecuteCrawl(string fundId)
        {
            DateTime beginTime = DateTime.Now;
            _url = _url + fundId;
            _browser.Load(_url);
            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > 3)
                {
                    _cts.Cancel();// need to implement inner logic
                    throw new Exception("crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }
            _browser.Dispose();
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
                    if (p != null && p.Url.Contains(_url))
                    {
                        List<Task> tasks = new List<Task>();
                        Thread.Sleep(1000*2);
                        string jscript1 = string.Format("$(\'.percentage\').text();");
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

            }, _ct);
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

    
}

