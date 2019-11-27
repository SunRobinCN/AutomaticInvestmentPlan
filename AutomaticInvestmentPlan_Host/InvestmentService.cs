using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Model;
using AutomaticInvestmentPlan_Model.Converter;
using AutomaticInvestmentPlan_Network;

namespace AutomaticInvestmentPlan_Host
{
    public class InvestmentService
    {
        //private readonly GeneralPointService _generalPointService = new GeneralPointService();
        //private readonly SpecifyFundJumpService _specifyFundJumpService = new SpecifyFundJumpService();
        //private readonly SpecifyFundHistoryJumpService _specifyFundHistoryJumpService = new SpecifyFundHistoryJumpService();
        //private readonly SpecifyFundNameService _specifyFundNameService = new SpecifyFundNameService();
        private readonly BuyService _buyService = new BuyService();


        public void Execute(string fundId)
        {
            //Task t1 = Task.Factory.StartNew(() =>
            //{
            //    FileLog.Info("Task1 started", LogType.Info);
            //    Console.WriteLine(@"Task1 started");
            //    string r = _generalPointService.ExecuteCrawl();
            //    GeneralPointModel m = NetworkValueConverter.ConvertToGeneralPointModel(r);
            //    CacheUtil.GeneralPoint = Convert.ToDouble(m.Point);
            //    FileLog.Info("Task1 ended with general point " + CacheUtil.GeneralPoint, LogType.Info);
            //    Console.WriteLine(@"Task1 ended with general point " + CacheUtil.GeneralPoint);
            //});

            //Task t2 = Task.Factory.StartNew(() =>
            //{
            //    FileLog.Info("Task2 started", LogType.Info);
            //    Console.WriteLine(@"Task2 started");
            //    string p = _specifyFundJumpService.ExecuteCrawl(fundId);
            //    CacheUtil.SpecifyEstimationJumpPoint = Convert.ToDouble(p.Substring(0, p.Length - 1)) / 100;
            //    FileLog.Info("Task2 ended with estimation jump result " + CacheUtil.SpecifyEstimationJumpPoint, LogType.Info);
            //    Console.WriteLine(@"Task2 ended with estimation jump result " + CacheUtil.SpecifyEstimationJumpPoint);
            //});

            //Task t3 = Task.Factory.StartNew(() =>
            //{
            //    FileLog.Info("Task3 started", LogType.Info);
            //    Console.WriteLine(@"Task3 started");
            //    string r = _specifyFundHistoryJumpService.ExecuteCrawl(fundId);
            //    List<HistoryPointModel> list = NetworkValueConverter.ConvertToHistoryPointModel(r);
            //    int count = 0;
            //    CacheUtil.SpecifyPointJumpHistory = new List<double>();
            //    foreach (HistoryPointModel historyPointModel in list)
            //    {
            //        if (count++ < 3)
            //        {
            //            CacheUtil.SpecifyPointJumpHistory.Add
            //                (Convert.ToDouble(historyPointModel.Point.Substring(0, historyPointModel.Point.Length - 1)) / 100);
            //        }
            //    }
            //    FileLog.Info("Task3 ended with history " + r, LogType.Info);
            //    Console.WriteLine(@"Task3 ended with history " + r);
            //});

            //Task t4 = Task.Factory.StartNew(() =>
            //{
            //    FileLog.Info("Task4 started", LogType.Info);
            //    Console.WriteLine(@"Task4 started");
            //    string name = _specifyFundNameService.ExecuteCrawl(fundId);
            //    CacheUtil.name = name;
            //    FileLog.Info("Task4 ended with name" + CacheUtil.name, LogType.Info);
            //    Console.WriteLine(@"Task4 ended with name" + CacheUtil.name);
            //});
            //List<Task> tasks = new List<Task>()
            //{
            //    t1,t2,t3,t4
            //};
            //FileLog.Info("Start to wait for all the tasks to be done", LogType.Info);
            //Task.WaitAll(tasks.ToArray());
            //FileLog.Info("All the tasks are done", LogType.Info);
            //double result = CalculateUtil.CalcuateInvestmentAmount(CacheUtil.GeneralPoint,
            //    CacheUtil.SpecifyEstimationJumpPoint, CacheUtil.SpecifyPointJumpHistory);
            //string subject = "Investment Reminder";
            //StringBuilder builder = new StringBuilder();
            //foreach (double d in CacheUtil.SpecifyPointJumpHistory)
            //{
            //    builder.Append(d*100 + "%  ");
            //}
            //string body =
            //    $"今日上证指数{CacheUtil.GeneralPoint}\r\n今日本基金预估涨跌{CacheUtil.SpecifyEstimationJumpPoint*100}%\r\n基金{CacheUtil.name}\r\n本期定投金额为{Math.Round(result)}\r\n本基金历史业绩{builder}";
            //EmailUtil.Send(subject, body);
            //Console.WriteLine(@"email is sent out");
            //FileLog.Info("email is sent out", LogType.Info);

            Task t5 = Task.Factory.StartNew(() =>
            {
                FileLog.Info("Task5 started", LogType.Info);
                Console.WriteLine(@"Task5 started");
                string r = _buyService.ExecuteBuy(11);
                FileLog.Info("Task5 ended with " + r, LogType.Info);
                Console.WriteLine(@"Task5 ended with " + r);
            });

            Task.WaitAll(new Task[] {t5});
            Console.WriteLine("done");
        }
    }
}