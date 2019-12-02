using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Network;
using Topshelf;

namespace AutomaticInvestmentPlan_Host
{
    public class ServiceManager
    {
        private bool _signal = true;

        private List<Task> _tasks = new List<Task>();

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

                //Task.Factory.StartNew(() =>
                //{
                //    while (_signal)
                //    {
                //        Debug.WriteLine("Task amount in total " + CacheUtil.Tasks.Count);
                //        FileLog.Info("Task amount in total " + CacheUtil.Tasks.Count, LogType.Info);
                //        Console.WriteLine(@"Task amount in total " + CacheUtil.Tasks.Count);
                //        StringBuilder builer = new StringBuilder();
                //        foreach (Task task in CacheUtil.Tasks)
                //        {
                //            builer.Append(task.Id + " " + task.Status + ",");
                //        }
                //        if (builer.Length > 0)
                //        {
                //            string r = builer.ToString(0, builer.Length - 1);
                //            Debug.WriteLine(r);
                //            FileLog.Debug(r, LogType.Debug);
                //            Console.WriteLine(r);
                //        }
                //        Thread.Sleep(1000 * 30);
                //    }
                //});

                Task t= Task.Factory.StartNew2(() =>
                {
                    try
                    {
                        InvestmentService investmentService = new InvestmentService();
                        WorkDayService workDayService = new WorkDayService();

                        TimeSpan start = new TimeSpan(14, 51, 0);
                        TimeSpan end = new TimeSpan(14, 52, 0);

                        while (_signal)
                        {
                            if (CheckWhetherInCorespondingTime(start, end) && workDayService.WhetherWorkDay())
                            {
                                FileLog.Info("Start to execute", LogType.Info);
                                investmentService.Execute("240014");
                            }
                            else
                            {
                                FileLog.Debug("skip this loop", LogType.Debug);
                            }
                            Thread.Sleep(1000 * 60 * 1);
                        }

                        //FileLog.Info("Start to execute", LogType.Info);
                        //investmentService.Execute("240014");
                        //Thread.Sleep(1000 * 60 * 20);
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
                //communicate with OS and stop the windows service gracefully
                int timeout = 20000;
                var shutDowntask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Task.WaitAll(_tasks.ToArray());
                        FileLog.Info("Sub worker threads have stopped", LogType.Info);
                        Console.WriteLine(@"Sub worker threads have stopped");
                    }
                    catch (Exception e)
                    {
                        FileLog.Error("ServiceManager.Stop", e, LogType.Error);
                        Console.WriteLine(e.Message);
                    }
                });
                while (!shutDowntask.Wait(timeout))
                {
                    FileLog.Info("Sub worker threads require another " + (timeout / 1000) + " seconds", LogType.Info);
                    Console.WriteLine($@"Sub worker threads require another {(timeout / 1000)} seconds");
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
