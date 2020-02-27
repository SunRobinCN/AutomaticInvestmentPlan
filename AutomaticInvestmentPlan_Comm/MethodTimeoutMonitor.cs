using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
                            CustomTimeoutException e = new CustomTimeoutException("time out") {Info = disposable.Name};
                            throw e;
                        }
                    }

                    Thread.Sleep(1000 * 1);
                }
            });
        }
    }
}