using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Model;
using AutomaticInvestmentPlan_Model.Converter;
using AutomaticInvestmentPlan_Network;

namespace AutomaticInvestmentPlan_Host
{
    public class InvestmentService : IDisposable
    {
        private readonly GeneralPointService _generalPointService = new GeneralPointService();
        private readonly SpecifyFundJumpService _specifyFundJumpService = new SpecifyFundJumpService();
        private readonly SpecifyFundHistoryJumpService _specifyFundHistoryJumpService = new SpecifyFundHistoryJumpService();
        private readonly SpecifyFundNameService _specifyFundNameService = new SpecifyFundNameService();
        private readonly SpecifyFundBuyService _specifyFundBuyService = new SpecifyFundBuyService();

        public void Execute(string fundId)
        {
            CacheUtil.RefrshCache();

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
                        this.Dispose();
                        Thread.Sleep(1000 * 30);
                        throw new CustomTimeoutException("time out");
                    }
                    Thread.Sleep(1000 * 1);
                }
            });

            Task t1 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    FileLog.Info("Task1 started", LogType.Info);
                    Console.WriteLine(@"Task1 started");
                    Debug.WriteLine("Task1 started");
                    string r = _generalPointService.ExecuteCrawl();
                    GeneralPointModel m = NetworkValueConverter.ConvertToGeneralPointModel(r);
                    CacheUtil.GeneralPoint = Convert.ToDouble(m.Point);
                    CacheUtil.GeneralPointJump = m.Percentate;
                    FileLog.Info("Task1 ended with general point " + CacheUtil.GeneralPoint, LogType.Info);
                    Console.WriteLine(@"Task1 ended with general point " + CacheUtil.GeneralPoint);
                    Debug.WriteLine("Task1 ended with general point " + CacheUtil.GeneralPoint);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }
            });

            Task t2 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    Thread.Sleep(1000);
                    FileLog.Info("Task2 started", LogType.Info);
                    Console.WriteLine(@"Task2 started");
                    Debug.WriteLine("Task2 started");
                    string p = _specifyFundJumpService.ExecuteCrawl(fundId);
                    CacheUtil.SpecifyEstimationJumpPoint = Convert.ToDouble(p.Substring(0, p.Length - 1)) / 100;
                    FileLog.Info("Task2 ended with estimation jump result " + CacheUtil.SpecifyEstimationJumpPoint, LogType.Info);
                    Console.WriteLine(@"Task2 ended with estimation jump result " + CacheUtil.SpecifyEstimationJumpPoint);
                    Debug.WriteLine("Task2 ended with estimation jump result " + CacheUtil.SpecifyEstimationJumpPoint);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }

            });

            Task t3 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    Thread.Sleep(1500);
                    FileLog.Info("Task3 started", LogType.Info);
                    Console.WriteLine(@"Task3 started");
                    Debug.WriteLine("Task3 started");
                    string r = _specifyFundHistoryJumpService.ExecuteCrawl(fundId);
                    List<HistoryPointModel> list = NetworkValueConverter.ConvertToHistoryPointModel(r);
                    int count = 0;
                    CacheUtil.SpecifyPointJumpHistory = new List<double>();
                    foreach (HistoryPointModel historyPointModel in list)
                    {
                        if (count++ < 3)
                        {
                            CacheUtil.SpecifyPointJumpHistory.Add
                                (Convert.ToDouble(historyPointModel.Point.Substring(0, historyPointModel.Point.Length - 1)) / 100);
                        }
                    }
                    FileLog.Info("Task3 ended with history " + r.Substring(0, 3), LogType.Info);
                    Console.WriteLine(@"Task3 ended with history " + r.Substring(0, 3));
                    Debug.WriteLine("Task3 ended with history " + r.Substring(0, 3));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }
            });

            Task t4 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    Thread.Sleep(2000);
                    FileLog.Info("Task4 started", LogType.Info);
                    Console.WriteLine(@"Task4 started");
                    Debug.WriteLine("Task4 started");
                    string name = _specifyFundNameService.ExecuteCrawl(fundId);
                    CacheUtil.Name = name;
                    FileLog.Info("Task4 ended with name" + CacheUtil.Name, LogType.Info);
                    Console.WriteLine(@"Task4 ended with name" + CacheUtil.Name);
                    Debug.WriteLine("Task4 ended with name" + CacheUtil.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }
            });

            List<Task> tasks = new List<Task>()
            {
                t1,t2,t3,t4
            };

            FileLog.Info("Start to wait for all the tasks to be done", LogType.Info);
            Debug.WriteLine("Start to wait for all the tasks to be done");
            Console.WriteLine(@"Start to wait for all the tasks to be done");
            Task.WaitAll(tasks.ToArray());
            FileLog.Info("All the tasks are done", LogType.Info);
            Debug.WriteLine("All the tasks are done");
            Console.WriteLine(@"All the tasks are done");
            double investAmount = CalculateUtil.CalcuateInvestmentAmount(CacheUtil.GeneralPoint,
                CacheUtil.SpecifyEstimationJumpPoint, CacheUtil.SpecifyPointJumpHistory);
            CacheUtil.BuyAmount = Math.Round(investAmount).ToString(CultureInfo.CurrentCulture);

            Task t5 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    if (investAmount > 0)
                    {
                        FileLog.Info("Task5 buy started with " + CacheUtil.BuyAmount + " amount", LogType.Info);
                        Console.WriteLine(@"Task5 buy started with " + CacheUtil.BuyAmount + @" amount");
                        Debug.WriteLine("Task5 buy started with " + CacheUtil.BuyAmount + " amount");
                        string r = _specifyFundBuyService.ExecuteBuy();
                        FileLog.Info("Task5 ended with " + r, LogType.Info);
                        Console.WriteLine(@"Task5 ended with " + r);
                        Debug.WriteLine("Task5 ended with " + r);
                        CacheUtil.BuyResult = r;
                        if (string.IsNullOrEmpty(r) == false)
                        {
                            FileLog.Info("Start to write signal file", LogType.Info);
                            FileUtil.WriteSingalToFile();
                        }
                    }
                    else
                    {
                        CacheUtil.BuyResult = "0";
                        FileUtil.WriteSingalToFile();
                        FileLog.Info("Not buy today", LogType.Info);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }
            });
            Task.WaitAll(t5);
            Console.WriteLine("done");
        }

        public void Dispose()
        {
            _generalPointService.Dispose();
            _specifyFundJumpService.Dispose();
            _specifyFundHistoryJumpService.Dispose();
            _specifyFundNameService.Dispose();
            _specifyFundBuyService.Dispose();
        }
    }
}