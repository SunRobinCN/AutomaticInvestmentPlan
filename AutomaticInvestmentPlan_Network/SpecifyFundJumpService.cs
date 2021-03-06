﻿using System;
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
    public class SpecifyFundJumpService : MyDisposable
    {
        private string _url = "http://fund.eastmoney.com/";
        private ChromiumWebBrowser _browser;
        private string _result;
        private bool _done;
        private Form _f;

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
            _browser.RequestHandler = new MyRequestHandler();
            Control.CheckForIllegalCrossThreadCalls = false;

            CombineLog.LogInfo("SpecifyFundJumpService class is constructed");
        }

        public string ExecuteCrawl(string fundId)
        {
            
            Task.Factory.StartNew(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (_f != null)
                {
                    CombineLog.LogInfo("start render form in SpecifyFundJumpService");
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
                if (midTime.TotalMinutes > Constant.TimeOutMinutes)
                {
                    _browser.Dispose();
                    _f.Dispose();
                    CombineLog.LogInfo("Dispose SpecifyFundJumpService for time out");
                    throw new Exception("SpecifyFundJumpService crawl time out");
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
                    if (e is FrameLoadEndEventArgs p && p.Url.Contains(_url))
                    {
                        Thread.Sleep(1000*30);
                        List<Task> tasks = new List<Task>();
                        string jscript0 = "$(\'span:contains(\"立即开启\")\').click();";
                        Task t = browser.EvaluateScriptAsync(jscript0);
                        Task.WaitAll(t);
                        Thread.Sleep(1000 * 10);
                        string jscript1 =
                            "var dates = [];var historys = [];var points = [];$(\'#Li1 tr\').each(function(i){                        $(this).children(\'td\').each(function(j){          if(j == 0) {            dates.push($(this).text());        }        if(j == 2) {            points.push($(this).text());        }        if(j == 3) {            historys.push($(this).text());        }      });});var combinedDates=\'\';var combinedHistory =\'\';var combinedPoints =\'\';$.each(dates,function(index,value){     combinedDates = combinedDates + \'^\' +value;});$.each(historys,function(index,value){     combinedHistory = combinedHistory + \'^\' +value;});$.each(points,function(index,value){     combinedPoints = combinedPoints + \'^\' +value;});var result = $(\'#gz_gszze\').text() + \"@\" + $(\'#gz_gszzl\').text() + \"@\" + combinedDates + \"@\" + combinedHistory + \"@\" + combinedPoints + \"@\" + $(\'.fundDetail-tit > div\').text();result;";
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
            CombineLog.LogInfo("Dispose SpecifyFundJumpService form");
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

