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
    public class InvestmentService
    {
        private readonly GeneralPointService _generalPointService = new GeneralPointService();
        private readonly SpecifyFundJumpService _specifyFundJumpService = new SpecifyFundJumpService();
        private readonly SpecifyFundHistoryJumpService _specifyFundHistoryJumpService = new SpecifyFundHistoryJumpService();
        private readonly SpecifyFundNameService _specifyFundNameService = new SpecifyFundNameService();
        private readonly BuyService _buyService = new BuyService();

        public void Execute(string fundId)
        {
            CacheUtil.RefrshCache();

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

            Task.WaitAll(tasks.ToArray());

            FileLog.Info("Start to wait for all the tasks to be done", LogType.Info);
            Debug.WriteLine("Start to wait for all the tasks to be done");
            Console.WriteLine(@"Start to wait for all the tasks to be done");
            Task.WaitAll(tasks.ToArray());
            FileLog.Info("All the tasks are done", LogType.Info);
            Debug.WriteLine("All the tasks are done");
            Console.WriteLine(@"All the tasks are done");
            double result = CalculateUtil.CalcuateInvestmentAmount(CacheUtil.GeneralPoint,
                CacheUtil.SpecifyEstimationJumpPoint, CacheUtil.SpecifyPointJumpHistory);
            CacheUtil.BuyAmount = Math.Round(result).ToString(CultureInfo.CurrentCulture);

            Task t5 = Task.Factory.StartNew2(() =>
            {
                try
                {
                    if (result > 0)
                    {
                        FileLog.Info("Task5 buy started with " + CacheUtil.BuyAmount + " amount", LogType.Info);
                        Console.WriteLine(@"Task5 buy started with " + CacheUtil.BuyAmount + @" amount");
                        Debug.WriteLine("Task5 buy started with " + CacheUtil.BuyAmount + " amount");
                        string r = _buyService.ExecuteBuy();
                        FileLog.Info("Task5 ended with " + r, LogType.Info);
                        Console.WriteLine(@"Task5 ended with " + r);
                        Debug.WriteLine("Task5 ended with " + r);
                        CacheUtil.BuyResult = r;
                    }
                    else
                    {
                        FileLog.Info("Not but today", LogType.Info);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    FileLog.Error("", e, LogType.Error);
                }
            });
            Task.WaitAll(t5);


            string subject = "Investment Reminder";
            StringBuilder builder = new StringBuilder();
            foreach (double d in CacheUtil.SpecifyPointJumpHistory)
            {
                builder.Append(d * 100 + "%  ");
            }
            string body =
                $"今日上证指数{CacheUtil.GeneralPoint}\r\n\r\n{CacheUtil.Name}\r\n今日本基金预估涨跌{CacheUtil.SpecifyEstimationJumpPoint * 100}%\r\n" +
                $"今日本期定投金额为{Math.Round(result)}\r\n本基金历史业绩{builder}\r\n" +
                $"今日本期定投结果为{CacheUtil.BuyResult}";
            EmailUtil.Send(subject, body);
            Console.WriteLine(@"email is sent out");
            FileLog.Info("email is sent out", LogType.Info);
            Debug.WriteLine("email is sent out");


            Console.WriteLine("done");
        }
    }
}