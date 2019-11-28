using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using Topshelf;

namespace AutomaticInvestmentPlan_Host
{
    class ServiceManager
    {

        private readonly List<Task> _tasks = new List<Task>();
        private bool _signal = true;

        public ServiceManager()
        {
             try
            {
            }
            catch (Exception ex)
            {
                FileLog.Error("ServiceManager.ServiceManager", ex, LogType.Error);
            }
        }


        private bool CheckWhetherInCorespondingTime(TimeSpan start, TimeSpan end)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if ((now > start) && (now < end))
            {
                return true;
            }
            return false;
        }

        public bool Start()
        {
            try
            {
                FileLog.Info("The service is starting", LogType.Info);
                Task t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        InvestmentService investmentService = new InvestmentService();
                        FileLog.Info("Start to execute", LogType.Info);
                        CacheUtil.BuyAmount = "11";
                        investmentService.Execute("240014");

                        //TimeSpan start = new TimeSpan(14, 50, 0);
                        //TimeSpan end = new TimeSpan(14, 52, 0);
                        //while (_signal)
                        //{
                        //    //if (CheckWhetherInCorespondingTime(start, end))
                        //    //{
                        //    //    FileLog.Info("Start to execute", LogType.Info);
                        //    //    investmentService.Execute("240014");
                        //    //}
                        //    //else
                        //    //{
                        //    //    FileLog.Debug("skip this loop", LogType.Debug);
                        //    //}

                        //    Thread.Sleep(1000 * 60 * 200);

                        //}
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        FileLog.Error("Start", e, LogType.Error);
                    }
                });
                _tasks.Add(t);
                FileLog.Info("The service is started", LogType.Info);
            }
            catch (Exception ex)
            {
                FileLog.Error("ServiceManager.Start", ex, LogType.Error);
            }

            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            try
            {
                FileLog.Info("The service is stopping", LogType.Info);
                _signal = false;
                // communicate with OS and stop the windows service gracefully
                int timeout = 20000;
                var shutDowntask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Task.WaitAll(_tasks.ToArray());
                        FileLog.Info("Sub worker threads have stopped", LogType.Info);
                    }
                    catch (Exception e)
                    {
                        FileLog.Error("ServiceManager.Stop", e, LogType.Error);
                    }
                });
                while (!shutDowntask.Wait(timeout))
                {
                    FileLog.Info("Sub worker threads require another " + (timeout/1000) + " seconds", LogType.Info);
                    Console.WriteLine("Sub worker threads require another " + (timeout / 1000) + " seconds");
                    hostControl.RequestAdditionalTime(TimeSpan.FromSeconds(timeout));
                }
                FileLog.Info("The service is stopped", LogType.Info);
            }
            catch (Exception ex)
            {
                FileLog.Error("Stop", ex, LogType.Error);
            }
            return true;
        }
    }
}
