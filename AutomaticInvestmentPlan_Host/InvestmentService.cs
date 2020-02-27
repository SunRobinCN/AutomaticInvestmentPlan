using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_DBService;
using AutomaticInvestmentPlan_Model;
using AutomaticInvestmentPlan_Network;

namespace AutomaticInvestmentPlan_Host
{
    public class InvestmentService : MyDisposable
    {
        private readonly GeneralPointService _generalPointService = new GeneralPointService();
        private readonly SpecifyFundJumpService _specifyFundJumpService = new SpecifyFundJumpService();
        private readonly SpecifyFundHistoryJumpService _specifyFundHistoryJumpService = new SpecifyFundHistoryJumpService();
        private readonly SpecifyFundNameService _specifyFundNameService = new SpecifyFundNameService();
        private readonly SpecifyFundBuyService _specifyFundBuyService = new SpecifyFundBuyService();
        private readonly SpecifyFundSellService _specifyFundSellService = new SpecifyFundSellService();

        private readonly DbService _dbService = new DbService();

        public InvestmentService()
        {
            CombineLog.LogInfo("SpecifyFundNameService class is constructed");
            Name = "InvestmentService";
        }

        public void ExecuteBuy(string fundId)
        {
            MethodTimeoutMonitor.TimeoutMonitor(this);
            ExecuteBuyTask(fundId);
            JobDone = true;
        }

        public void ExecuteSell(string fundId)
        {
            MethodTimeoutMonitor.TimeoutMonitor(this);
            ExecuteSellTask(fundId);
            JobDone = true;
        }

        private void ExecuteBuyTask(string fundId)
        {
            GeneralPointModel generalPointModel = null;
            double generalPoint = 0;
            string fundName = null;
            double estimationJump = 0;
            double estimationValue = 0;
            List<double> jumpHistory = null;
            Task t1 = Task.Factory.StartNew(() =>
            {
                generalPointModel = RunTaskForGetGeneralPoint();
                generalPoint = Convert.ToDouble(generalPointModel.Point);
            });
            Task t2 = Task.Factory.StartNew(() =>
            {
                fundName = RunTaskForGetFundName(fundId);
            });
            Task t3 = Task.Factory.StartNew(() =>
            {
                string r = RunTaskForGetFundEstimationJump(fundId);
                string[] sarray = r.Split('|');
                estimationJump = Convert.ToDouble(sarray[0].Substring(0, sarray[0].Length - 1)) / 100;
                estimationValue = Convert.ToDouble(sarray[1]);
            });
            Task t4 = Task.Factory.StartNew(() =>
            {
                jumpHistory = RunTaskForGetFundJumpHistory(fundId);
            });

            CombineLog.LogInfo("Start to wait for all the tasks to be done");
            List<Task> tasks = new List<Task>()
            {
                t1,t2,t3,t4
            };
            Task.WaitAll(tasks.ToArray());
            CombineLog.LogInfo("All the tasks are done");
            CacheUtil.AddFundNameInCache(fundId, fundName);
            CacheUtil.GetFundDetailInCache(fundId).EstimationFundValue = estimationValue;
            CacheUtil.GetFundDetailInCache(fundId).EstimationJumpPoint = estimationJump;
            double investAmount = CalculateUtil.CalculateInvestmentAmount(fundId, generalPoint, estimationJump, jumpHistory);
            CacheUtil.BuyAmount = Math.Round(investAmount).ToString(CultureInfo.CurrentCulture);
            CacheUtil.GetFundDetailInCache(fundId).BuyAmount = CacheUtil.BuyAmount;
            var buyResult = RunTaskForBuyFund(fundId, investAmount);
            if (string.IsNullOrEmpty(buyResult) == false)
            {
                CacheUtil.GetFundDetailInCache(fundId).BuyResult = buyResult;
                CombineLog.LogInfo($"Buy {fundId} {fundName} is OK. Start to write database");
                HistoryModel historyModel = new HistoryModel
                {
                    FundId = fundId,
                    BuyDate = DateTime.Now,
                    FundName = fundName,
                    ShangHaiIndex = generalPoint,
                    ShangHaiIndexJumpPercentage = generalPointModel.Percentate,
                    FundValue = estimationValue,
                    FundValueJumpPercentage = estimationJump,
                    BuyAmount = investAmount
                };
                _dbService.InsertBuyResult(historyModel);
            }
            CombineLog.LogInfo($"Buy {fundId} {fundName} down");
        }

        private void ExecuteSellTask(string fundId)
        {
            string fundName = CacheUtil.GetFundNameInCache(fundId);
            double fundValue = CacheUtil.GetFundDetailInCache(fundId).EstimationFundValue;
            List<HistoryModel> list = _dbService.SelectAllNotSold(fundId);
            List<HistoryModel> sellList = new List<HistoryModel>();
            foreach (HistoryModel historyModel in list)
            {
                if ((fundValue - historyModel.FundValue) / historyModel.FundValue >
                    CacheUtil.IncreasePercentage  )
                {
                    historyModel.FundShare = historyModel.BuyAmount / historyModel.FundValue;
                    CombineLog.LogInfo($"The record with id {historyModel.FundId} will be sold");
                    sellList.Add(historyModel);
                }
            }
            int sellShareAmount = Convert.ToInt32(sellList.Sum(t => t.FundShare));
            CacheUtil.GetFundDetailInCache(fundId).SellShareAmount = sellShareAmount.ToString();
            CombineLog.LogInfo($"fund {fundId} total share amount is {sellShareAmount}");
            var sellResult = RunTaskForSellFund(fundId, sellShareAmount);
            if (string.IsNullOrEmpty(sellResult) == false)
            {
                CacheUtil.GetFundDetailInCache(fundId).SellResult = sellResult;
                CombineLog.LogInfo($"Sell {fundId} {sellShareAmount} is OK. Start to write database");

                foreach (HistoryModel model in sellList)
                {
                    model.AlreaySold = 1;
                    model.FundValueInSell = fundValue;
                    model.SellDate = DateTime.Now;
                    model.SellAmount = Math.Round(model.FundValueInSell * model.FundShare, 2);
                    model.Profit = Math.Round(model.SellAmount - model.BuyAmount, 2);
                    model.ProfitPercentage = Math.Round(model.Profit / model.BuyAmount, 2);
                    _dbService.UpdateSellResult(model);
                }
            }
            CombineLog.LogInfo($"Sell {fundId} {fundName} down");

        }


        private GeneralPointModel RunTaskForGetGeneralPoint()
        {
            GeneralPointCache generalPointCache = CacheUtil.GetGeneralPointInCache(DateTime.Now.ToString("yyyy-MM-dd"));
            if (Math.Abs(generalPointCache.GeneralPoint) > 0.01)
            {
                GeneralPointModel generalPointModel = new GeneralPointModel
                {
                    Point = Convert.ToString(generalPointCache.GeneralPoint, CultureInfo.InvariantCulture),
                    Percentate = generalPointCache.GeneralPointJump
                };
                return generalPointModel;
            }

            GeneralPointModel result = new GeneralPointModel();
            try
            {
                Thread.Sleep(2000);
                CombineLog.LogInfo("Task RunTaskForGetGeneralPoint started");
                var strResult = _generalPointService.ExecuteCrawl();
                result = NetworkValueConverter.ConvertToGeneralPointModel(strResult);
                CombineLog.LogInfo("Task RunTaskForGetGeneralPoint ended");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetGeneralPoint", e, LogType.Error);
            }
            return result;
        }

        private string RunTaskForGetFundName(string fundId)
        {
            string cacheName = CacheUtil.GetFundNameInCache(fundId);
            if (!string.IsNullOrEmpty(cacheName))
            {
                return cacheName;
            }
            string result = "";
            try
            {
                CombineLog.LogInfo("Task RunTaskForGetFundName " + fundId + " started");
                result = _specifyFundNameService.ExecuteCrawl(fundId);
                CombineLog.LogInfo("Task RunTaskForGetFundName " + fundId + " ended");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetFundName", e, LogType.Error);
            }
            return result;
        }

        private string RunTaskForGetFundEstimationJump(string fundId)
        {
            string result = "";
            try
            {
                Thread.Sleep(2000);
                CombineLog.LogInfo("Task RunTaskForGetFundEstimationJump " + fundId + " started");
                result = _specifyFundJumpService.ExecuteCrawl(fundId);
                CombineLog.LogInfo("Task RunTaskForGetFundEstimationJump " + fundId + " ended");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetFundEstimationJump", e, LogType.Error);
            }
            return result;
        }

        private List<double> RunTaskForGetFundJumpHistory(string fundId)
        {
            List<double> result = new List<double>();
            try
            {
                Thread.Sleep(2000);
                CombineLog.LogInfo("Task RunTaskForGetFundJumpHistory started");
                string r = _specifyFundHistoryJumpService.ExecuteCrawl(fundId);
                List<HistoryPointModel> historyPointModels = NetworkValueConverter.ConvertToHistoryPointModel(r);
                int count = 0;
                foreach (HistoryPointModel historyPointModel in historyPointModels)
                {
                    if (count++ < 3)
                    {
                        result.Add(Convert.ToDouble(
                                       historyPointModel.Point.Substring(0, historyPointModel.Point.Length - 1)) / 100);
                    }
                }
                CombineLog.LogInfo("Task RunTaskForGetFundJumpHistory ended");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetFundJumpHistory", e, LogType.Error);
            }
            return result;
        }

        private string RunTaskForBuyFund(string fundId, double investAmount)
        {
            string result = "";
            try
            {
                Thread.Sleep(2000);
                if (investAmount > 0)
                {
                    CombineLog.LogInfo("Task RunTaskForBuyFund " + fundId + " " + investAmount + " started");
                    result = _specifyFundBuyService.ExecuteBuy(fundId, investAmount.ToString(CultureInfo.InvariantCulture));
                    CombineLog.LogInfo("Task RunTaskForBuyFund " + fundId + " " + investAmount + " ended");
                }
                else
                {
                    CombineLog.LogInfo("Not buy today for " + fundId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetFundEstimationJump", e, LogType.Error);
            }
            return result;
        }

        private string RunTaskForSellFund(string fundId, double sellAmount)
        {
            string result = "";
            try
            {
                Thread.Sleep(2000);
                if (sellAmount > 0)
                {
                    CombineLog.LogInfo("Task RunTaskForSellFund " + fundId + " " + sellAmount + " started");
                    result = _specifyFundSellService.ExecuteSell(fundId, sellAmount.ToString(CultureInfo.InvariantCulture));
                    CombineLog.LogInfo("Task RunTaskForSellFund " + fundId + " " + sellAmount + " ended");
                }
                else
                {
                    CombineLog.LogInfo("Not sell today for " + fundId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                FileLog.Error("RunTaskForGetFundEstimationJump", e, LogType.Error);
            }
            return result;
        }

        public override void Dispose()
        {
            _generalPointService.Dispose();
            _specifyFundJumpService.Dispose();
            _specifyFundHistoryJumpService.Dispose();
            _specifyFundNameService.Dispose();
            _specifyFundBuyService.Dispose();
            _specifyFundSellService.Dispose();
        }
    }
}