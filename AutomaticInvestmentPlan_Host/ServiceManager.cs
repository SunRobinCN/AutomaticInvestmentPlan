using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_DBService;
using Topshelf;

namespace AutomaticInvestmentPlan_Host
{
    public class ServiceManager
    {
        private bool _signal = true;
        private readonly DbService _dbService = new DbService();

        private bool CheckWhetherInCorespondingTime(TimeSpan start, TimeSpan end)
        {
            if(System.Configuration.ConfigurationManager.AppSettings["SkipTimeSpanCheck"] == "true")
            {
                return true;
            }
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
                CombineLog.LogInfo("The service is starting");
                CacheUtil.AddStrategyFund("160706");
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        TimeSpan start = new TimeSpan(14, 41, 0);
                        TimeSpan end = new TimeSpan(14, 42, 0);

                        while (_signal)
                        {
                            if (CheckWhetherInCorespondingTime(start, end) && DayUtil.WhetherWeekend() == false
                                && DayUtil.WhetherHoliday() == false)
                            {
                                try
                                {
                                    SmsUtil.Send("819146", null, new[] { "+8618526088356" });
                                    CacheUtil.RefrshCache();

                                    BackupDB();

                                    //华宝中证100 240014
                                    DoExecuteBuy("240014");
                                    Thread.Sleep(1000 * 60 * 1);
                                    //嘉实沪深300 160706
                                    DoExecuteBuy("160706");
                                    Thread.Sleep(1000 * 60 * 1);
                                    //富国中证500 161017
                                    DoExecuteBuy("161017");

                                    SendOutFinalSms();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    FileLog.Error("Start.Loop", e, LogType.Error);
                                    SendOutErrorSms();
                                }
                            }
                            else
                            {
                                FileLog.Debug("skip this loop", LogType.Debug);
                            }
                            Thread.Sleep(1000 * 60 * 1);
                        }
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

        private void DoExecuteBuy(string fundId)
        {
            int count = 0;
            while (true)
            {
                if (_dbService.SelectBuyResultByDate(fundId, DateTime.Now.ToString("yyyy-MM-dd")) != null)
                {
                    CombineLog.LogInfo(fundId + " already executed");
                    break;
                }
                if (count++ > 2)
                {
                    CombineLog.LogInfo(fundId + " more than twice, skip");
                    break;
                }

                InvestmentService investmentService = new InvestmentService(fundId);
                try
                {
                    CombineLog.LogInfo("Start to execute buy " + fundId + " count " + count);
                    investmentService.ExecuteBuy();
                }
                catch (Exception e)
                {
                    CombineLog.LogError("DoExecuteBuy", e);
                }
                finally
                {
                    investmentService.Dispose();
                }
            }

        }

        private void DoExecuteSell(string fundId)
        {

            InvestmentService investmentService = new InvestmentService(fundId);
            try
            {
                CombineLog.LogInfo("Start to execute sell " + fundId);
                investmentService.ExecuteSell();
            }
            catch (Exception e)
            {
                CombineLog.LogError("DoExecuteSell", e);
            }
            finally
            {
                investmentService.Dispose();
            }
        }

        private void BackupDB()
        {
            string originalPath = "DB\\investment.db";
            string destinationPath = "DB\\backup\\";
            string destinationName = DateTime.Now.ToString("yyyy-MM-dd");
            FileUtil.BackUpFile(originalPath, destinationPath, destinationName);
        }

        private void SendOutFinalEmail()
        {
            string subject = "Investment Reminder";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder body = new StringBuilder();
            body.Append($"今日上证指数{CacheUtil.GetGeneralPointInCache(date).GeneralPoint}\r\n" +
                        $"今日上证涨跌{" " + CacheUtil.GetGeneralPointInCache(date).GeneralPointJump}\r\n\r\n");

            foreach (SpecifyFundCache specifyFundCache in CacheUtil.GetAllCaches())
            {
                StringBuilder builder = new StringBuilder();
                foreach (double d in specifyFundCache.SpecifyPointJumpHistory ?? new List<double>())
                {
                    builder.Append(d * 100 + "%  ");
                }

                string sellShare = string.IsNullOrEmpty(specifyFundCache.SellShareAmount)
                    ? "0"
                    : specifyFundCache.SellShareAmount;
                string sellResult = string.IsNullOrEmpty(specifyFundCache.SellResult)
                    ? "0"
                    : specifyFundCache.SellResult;
                body.Append(
                    $"\r\n\r\n{specifyFundCache.Name + ")"}\r\n今日本基金预估涨跌{specifyFundCache.EstimationJumpPercentage * 100}%\r\n" +
                    $"今日本期定投金额为{specifyFundCache.BuyAmount}\r\n本基金历史业绩{builder}\r\n" +
                    $"今日本期定投结果为{specifyFundCache.BuyResult}\r\n" +
                    $"今日本期卖出份额为{sellShare}\r\n" +
                    $"今日本期卖出结果为{sellResult}");

                body.Append("\r\n");
            }
            CombineLog.LogInfo("Start to send final email");
            EmailUtil.Send(subject, body.ToString());
            CombineLog.LogInfo("Final email is sent out");
        }

        private void SendOutFinalSms()
        {
            CombineLog.LogInfo("Start to send final sms");
            List<SpecifyFundCache> list = CacheUtil.GetAllCaches();
            var sum = list.Sum(s => Convert.ToDouble(s.BuyAmount)).ToString(CultureInfo.InvariantCulture);
            SmsUtil.Send("819346", new []{ sum }, new []{ "+8618526088356" });
            CombineLog.LogInfo("Final sms is sent out");
        }

        private void SendOutErrorSms()
        {
            CombineLog.LogInfo("Start to send error sms");
            List<SpecifyFundCache> list = CacheUtil.GetAllCaches();
            var sum = "Error";
            SmsUtil.Send("819346", new[] { sum }, new[] { "+8618526088356" });
            CombineLog.LogInfo("Error sms is sent out");
        }


        public bool Stop(HostControl hostControl)
        {
            try
            {
                CombineLog.LogInfo("The service is stopping");
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
