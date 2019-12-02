﻿using System;
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
    public class SpecifyFundHistoryJumpService
    {
        private string _url = "https://danjuanapp.com/net-history/";
        private readonly ChromiumWebBrowser _browser;
        private string _result;
        private bool _done;
        private readonly Form _f;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

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
            _f = new MountForm();
            _f.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.FrameLoadEnd += OnFrameLoadEnd;
            _browser.LoadError += OnLoadError;
            _browser.ConsoleMessage += OnConsoleMessage;
            _browser.JsDialogHandler = new JsDialogHandler();
            Control.CheckForIllegalCrossThreadCalls = false;

            FileLog.Info("SpecifyFundHistoryJumpService class is constructed", LogType.Info);
            Debug.WriteLine("SpecifyFundHistoryJumpService class is constructed");
        }

        public string ExecuteCrawl(string fundId)
        {
            Task.Factory.StartNew2(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    FileLog.Info("start render form in SpecifyFundHistoryJumpService", LogType.Info);
                    Debug.WriteLine("start render form in SpecifyFundHistoryJumpService");
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
            _url = _url + fundId;
            _browser.Load(_url);
            while (_done == false)
            {
                TimeSpan midTime = DateTime.Now - beginTime;
                if (midTime.TotalMinutes > 3)
                {
                    _tokenSource.Cancel();
                    _browser.Dispose();
                    _f.Dispose();
                    throw new Exception("SpecifyFundHistoryJumpService crawl time out");
                }
                Thread.Sleep(1000 * 2);
            }
            _tokenSource.Cancel();
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

