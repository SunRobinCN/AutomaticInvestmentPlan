using System;

namespace AutomaticInvestmentPlan_Comm
{
    public static class CustomDisposeUtil
    {
        public static void Dispose(IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (ObjectDisposedException e)
            {
                FileLog.Warn("DisposeWarn", e, LogType.Warn);
            }
        }
    }
}