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
                Task.Factory.StartNew2(() =>
                {
                    try
                    {
                        WorkDayService workDayService = new WorkDayService();

                        TimeSpan start = new TimeSpan(14, 50, 0);
                        TimeSpan end = new TimeSpan(14, 51, 0);

                        while (_signal)
                        {
                            if (CheckWhetherInCorespondingTime(start, end) && workDayService.WhetherWorkDay())
                            {
                                string subject = "Investment Reminder";
                                EmailUtil.Send(subject, "今日定投提醒\r\n\r\n 即将进行今日的定投扣款\r\n 请关注定投结果……");

                                DoExecute();

                                StringBuilder builder = new StringBuilder();
                                foreach (double d in CacheUtil.SpecifyPointJumpHistory ?? new List<double>())
                                {
                                    builder.Append(d * 100 + "%  ");
                                }
                                string body =
                                    $"今日上证指数{CacheUtil.GeneralPoint}\r\n\r\n{CacheUtil.Name}\r\n今日本基金预估涨跌{CacheUtil.SpecifyEstimationJumpPoint * 100}%\r\n" +
                                    $"今日本期定投金额为{CacheUtil.BuyAmount}\r\n本基金历史业绩{builder}\r\n" +
                                    $"今日本期定投结果为{CacheUtil.BuyResult}";
                                EmailUtil.Send(subject, body);
                                Console.WriteLine(@"email is sent out");
                                FileLog.Info("email is sent out", LogType.Info);
                                Debug.WriteLine("email is sent out");

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
                FileLog.Info("The service is started", LogType.Info);
            }
            catch (Exception ex)
            {
                FileLog.Error("ServiceManager.Start", ex, LogType.Error);
            }

            return true;
        }

        private static void DoExecute()
        {
            int count = 0;
            bool signal = true;
            while (signal)
            {
                if (FileUtil.ReadSingalFromFile())
                {
                    break;
                }
                if (count++ > 2)
                {
                    break;
                }

                InvestmentService investmentService = new InvestmentService();
                try
                {
                    FileLog.Info("Start to execute", LogType.Info);
                    investmentService.Execute("240014");
                    signal = false;
                }
                catch (Exception e)
                {
                    CombineLog.LogError("DoExecute", e);
                }
                finally
                {
                    investmentService.Dispose();
                }
            }
            
        }


        public bool Stop(HostControl hostControl)
        {
            try
            {
                FileLog.Info("The service is stopping", LogType.Info);
                _signal = false;
                DateTime beginTime = DateTime.Now;
                //communicate with OS and stop the windows service gracefully
                int timeout = 20000;
                var shutDowntask = Task.Factory.StartNew(() =>
                {
                    try
                    {
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
                    TimeSpan midTime = DateTime.Now - beginTime;
                    if (midTime.TotalMinutes > 3)
                    {
                        break;
                    }
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
