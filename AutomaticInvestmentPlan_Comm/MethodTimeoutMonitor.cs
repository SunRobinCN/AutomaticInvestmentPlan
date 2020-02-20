using System;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Host;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_Comm
{
    public class MethodTimeoutMonitor
    {
        public static void TimeoutMonitor(MyDisposable disposable)
        {
            bool executeTimeout = false;
            DateTime beginTime = DateTime.Now;
            Task.Factory.StartNew(() =>
            {
                while (executeTimeout == false)
                {
                    TimeSpan midTime = DateTime.Now - beginTime;
                    if (midTime.TotalMinutes > 5)
                    {
                        executeTimeout = true;
                        if (disposable.JobDone == false)
                        {
                            disposable.Dispose();
                            Thread.Sleep(1000 * 30);
                            throw new CustomTimeoutException("time out");
                        }
                    }

                    Thread.Sleep(1000 * 1);
                }
            });
        }
    }
}