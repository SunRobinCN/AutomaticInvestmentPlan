using System;
using System.Diagnostics;
using AutomaticInvestmentPlan_Comm;
using CefSharp;
using CefSharp.Handler;

namespace AutomaticInvestmentPlan_Network
{
    public class MyRequestHandler : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new ResourceRequestHandlerExt();
        }
    }

    public class ResourceRequestHandlerExt : ResourceRequestHandler
    {
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (request.Url.Contains("https://danjuanapp.com/djapi/v4/fund/order/buy"))
             {
                if (request.PostData != null)
                {
                    string str = System.Text.Encoding.UTF8.GetString(request.PostData.Elements[0].Bytes);
                    FileLog.Info("OnBeforeResourceLoad is invoked with amount " + CacheUtil.BuyAmount, LogType.Info);
                    if (Convert.ToInt32(CacheUtil.BuyAmount)>1000)
                    {
                        FileLog.Info("Amount is not OK", LogType.Info);
                        return base.OnBeforeResourceLoad(chromiumWebBrowser, browser, frame, request, callback);
                    }
                    string updatedContent = str.Replace("\"amount\":\"\"", $"\"amount\":\"{CacheUtil.BuyAmount}\"");
                    Byte[] updatedBytes = System.Text.Encoding.UTF8.GetBytes(updatedContent);
                    request.PostData.Elements[0].Bytes = updatedBytes;
                }
            }

            return base.OnBeforeResourceLoad(chromiumWebBrowser, browser, frame, request, callback);
        }
    }
}
