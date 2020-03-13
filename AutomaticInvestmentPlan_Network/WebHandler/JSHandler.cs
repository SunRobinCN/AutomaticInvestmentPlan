using CefSharp;

namespace AutomaticInvestmentPlan_Network.WebHandler
{
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
