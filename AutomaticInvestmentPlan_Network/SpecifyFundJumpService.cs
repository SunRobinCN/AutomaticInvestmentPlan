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
    public class SpecifyFundJumpService
    {
        private string _url = "http://fund.eastmoney.com/";
        private readonly ChromiumWebBrowser _browser;
        private string _result;
        private bool _done;
        private readonly Form _f;

        public SpecifyFundJumpService()
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

            FileLog.Info("SpecifyFundJumpService class is constructed", LogType.Info);
            Debug.WriteLine("SpecifyFundJumpService class is constructed");
        }

        public string ExecuteCrawl(string fundId)
        {
            
            Task.Factory.StartNew2(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    FileLog.Info("start render form in SpecifyFundJumpService", LogType.Info);
                    Debug.WriteLine("start render form in SpecifyFundJumpService");
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
            _url = _url + fundId + ".html";
            _browser.Load(_url);
            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > 3)
                {
                    _browser.Dispose();
                    _f.Dispose();
                    throw new Exception("SpecifyFundJumpService crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }
            _browser.Dispose();
            _f.Dispose();
            return _result;
        }

        void OnFrameLoadEnd(object sender, EventArgs e)
        {
            Task.Factory.StartNew2(() =>
            {
                try
                {
                    ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
                    if (e is FrameLoadEndEventArgs p && p.Url.Contains(_url))
                    {
                        Thread.Sleep(1000*30);
                        List<Task> tasks = new List<Task>();
                        string jscript1 = "document.getElementById(\'gz_gszzl\').innerText;";
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
        }

        void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
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

