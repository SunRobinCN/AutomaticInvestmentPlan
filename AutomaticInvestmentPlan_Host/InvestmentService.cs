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
        private readonly SpecifyFundBuyService _specifyFundBuyService = new SpecifyFundBuyService();
        private readonly SpecifyFundSellService _specifyFundSellService = new SpecifyFundSellService();

        private readonly DbService _dbService = new DbService();

        private readonly string _fundId;

        public InvestmentService(string fundId)
        {
            this._fundId = fundId;
            CombineLog.LogInfo("SpecifyFundNameService class is constructed for " + _fundId);
            Name = "InvestmentService";
        }

        public void ExecuteBuy()
        {
            //MethodTimeoutMonitor.TimeoutMonitor(this);
            ExecuteBuyTask();
            JobDone = true;
        }

        public void ExecuteSell()
        {
            //MethodTimeoutMonitor.TimeoutMonitor(this);
            ExecuteSellTask();
            JobDone = true;
        }

        private void ExecuteBuyTask()
        {
            CombineLog.LogInfo("Start to execute " + _fundId);
            CombinedResult calculateResult = ExecuteCalculateTask(_fundId);
            string fundName = calculateResult.FundName;
            GeneralPointModel generalPointModel = calculateResult.GeneralPoint;
            double generalPoint = Convert.ToDouble(generalPointModel.Point);
            double estimationJumpPercentage = calculateResult.EstimationJumpPercentage;
            double estimationValue = calculateResult.EstimationValue;
            double investAmount = calculateResult.InvestAmount;
            CacheUtil.BuyAmount = Math.Round(investAmount).ToString(CultureInfo.CurrentCulture);
            CacheUtil.GetFundDetailInCache(_fundId).BuyAmount = CacheUtil.BuyAmount;
            var buyResult = RunTaskForBuyFund(_fundId, investAmount);
            if (string.IsNullOrEmpty(buyResult) == false)
            {
                CacheUtil.GetFundDetailInCache(_fundId).BuyResult = buyResult;
                CombineLog.LogInfo($"Buy {_fundId} {fundName} is OK. Start to write database");
                HistoryModel historyModel = new HistoryModel
                {
                    FundId = _fundId,
                    BuyDate = DateTime.Now,
                    FundName = fundName,
                    ShangHaiIndex = generalPoint,
                    ShangHaiIndexJumpPercentage = generalPointModel.Percentate,
                    FundValue = estimationValue,
                    FundValueJumpPercentage = estimationJumpPercentage,
                    BuyAmount = investAmount
                };
                _dbService.InsertBuyResult(historyModel);
            }
            CombineLog.LogInfo($"Buy {_fundId} {fundName} done");
        }

        private CombinedResult ExecuteCalculateTask(string fundId)
        {
            GeneralPointModel generalPointModel = null;
            double generalPoint = 0;
            string fundName = null;
            double estimationJumpPercentage = 0;
            double estimationValue = 0;
            List<double> jumpHistory = new List<double>();
            List<double> accumulatedPointHistory = new List<double>();
            Task t1 = Task.Factory.StartNew(() =>
            {
                generalPointModel = RunTaskForGetGeneralPoint();
                generalPoint = Convert.ToDouble(generalPointModel.Point);
            });
            Task t3 = Task.Factory.StartNew(() =>
            {
                string r = RunTaskForGetFundEstimationJump(fundId);
                CombineLog.LogInfo("Get combined history result from browser " + r);
                string todayJumpPoint = r.Split('@')[0].Trim();
                string todayJumpPercentage = r.Split('@')[1].Trim();
                string historyDate = r.Split('@')[2];
                string historyJumpPercentage = r.Split('@')[3];
                string historyPointValue = r.Split('@')[4];
                string name = r.Split('@')[5];
                if (name.Contains("(") && !name.Contains(")"))
                {
                    fundName = name + ")";
                }
                else
                {
                    fundName = name;
                }
                CombineLog.LogInfo($"todayJumpPoint {todayJumpPoint} todayJumpPercentage {todayJumpPercentage} historyDate {historyDate} historyJumpPercentage" +
                    $" {historyJumpPercentage} historyPointValue {historyPointValue} fundName {fundName}");
                List<HistoryPointModel> list = NetworkValueConverter.
                    ConvertToHistoryPointModel(historyDate, historyJumpPercentage, historyPointValue);

                estimationJumpPercentage = Convert.ToDouble(todayJumpPercentage.Substring(0, todayJumpPercentage.Length - 2)) / 100;
                estimationValue = list[0].PointValue + Convert.ToDouble(todayJumpPoint);

                foreach (HistoryPointModel historyPointModel in list)
                {
                    accumulatedPointHistory.Add(historyPointModel.PointValue);
                    jumpHistory.Add(Convert.ToDouble(historyPointModel.PointJumpPercentage.Substring(0, historyPointModel.PointJumpPercentage.Length - 2)) / 100);
                }
            });

            CombineLog.LogInfo("Start to wait for all the tasks to be done");
            List<Task> tasks = new List<Task>()
            {
                t1,t3
            };
            Task.WaitAll(tasks.ToArray());
            CombineLog.LogInfo("All the tasks are done");
            CacheUtil.GetFundDetailInCache(fundId).EstimationFundValue = estimationValue;
            CacheUtil.GetFundDetailInCache(fundId).EstimationJumpPercentage = estimationJumpPercentage;
            CacheUtil.GetFundDetailInCache(fundId).Name = fundName;
            CacheUtil.GetFundDetailInCache(fundId).AcculatedPointHistory = accumulatedPointHistory;
            CacheUtil.GetFundDetailInCache(fundId).SpecifyPointJumpHistory = jumpHistory;
            CacheUtil.AddFundNameInCache(fundId, fundName);
            double investAmount = CalculateUtil.CalculateInvestmentAmount(fundId, estimationValue, generalPoint, estimationJumpPercentage, jumpHistory, accumulatedPointHistory);
            CombinedResult result = new CombinedResult
            {
                GeneralPoint = generalPointModel,
                FundName = fundName,
                EstimationValue = estimationValue,
                EstimationJumpPercentage = estimationJumpPercentage,
                InvestAmount = Math.Round(investAmount)
            };
            return result;
        }

        private void ExecuteSellTask()
        {
            string fundName = CacheUtil.GetFundNameInCache(_fundId);
            double fundValue = CacheUtil.GetFundDetailInCache(_fundId).EstimationFundValue;
            List<double> accumulatedPointHistory = CacheUtil.GetFundDetailInCache(_fundId).AcculatedPointHistory;
            List<HistoryModel> list = _dbService.SelectAllNotSold(_fundId);
            List<HistoryModel> sellList = new List<HistoryModel>();
            foreach (HistoryModel historyModel in list)
            {
                if (CalculateUtil.CalculateWhetherSell(historyModel, accumulatedPointHistory, fundValue))
                {
                    historyModel.FundShare = historyModel.BuyAmount / historyModel.FundValue;
                    CombineLog.LogInfo($"The record with id {historyModel.FundId} will be sold");
                    sellList.Add(historyModel);
                }
            }

            if (sellList.Count == 0)
            {
                return;
            }
            int sellShareAmount = Convert.ToInt32(sellList.Sum(t => t.FundShare));
            CacheUtil.GetFundDetailInCache(_fundId).SellShareAmount = sellShareAmount.ToString();
            CombineLog.LogInfo($"fund {_fundId} total share amount is {sellShareAmount}");
            if (CacheUtil.EftList.Contains(_fundId))
            {
                WriteResultInDb(_fundId, "", sellShareAmount, sellList, fundValue);
                return;
            }

            var sellResult = RunTaskForSellFund(_fundId, sellShareAmount);
            if (string.IsNullOrEmpty(sellResult) == false)
            {
                WriteResultInDb(_fundId, sellResult, sellShareAmount, sellList, fundValue);
            }
            CombineLog.LogInfo($"Sell {_fundId} {fundName} down");

        }

        private void WriteResultInDb(string fundId, string sellResult, int sellShareAmount, List<HistoryModel> sellList, double fundValue)
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


        private GeneralPointModel RunTaskForGetGeneralPoint()
        {
            GeneralPointCache generalPointCache = CacheUtil.GetGeneralPointInCache(DateTime.Now.ToString("yyyy-MM-dd"));
            if (Math.Abs(generalPointCache.GeneralPoint) > 0.01)
            {
                CombineLog.LogInfo("Already get general point in cache");
                GeneralPointModel generalPointModel = new GeneralPointModel
                {
                    Point = Convert.ToString(generalPointCache.GeneralPoint, CultureInfo.InvariantCulture),
                    Percentate = generalPointCache.GeneralPointJump
                };
                return generalPointModel;
            }

            GeneralPointModel result = new GeneralPointModel();
            Thread.Sleep(2000);
            CombineLog.LogInfo("Task RunTaskForGetGeneralPoint started");
            var strResult = _generalPointService.ExecuteCrawl();
            CombineLog.LogInfo("Get combined general point " + strResult);
            result = NetworkValueConverter.ConvertToGeneralPointModel(strResult);
            CombineLog.LogInfo("Task RunTaskForGetGeneralPoint ended");
            generalPointCache.GeneralPoint = Convert.ToDouble(result.Point);
            generalPointCache.GeneralPointJump = result.Percentate;
            return result;
        }

        private string RunTaskForGetFundEstimationJump(string fundId)
        {
            string result = "";
            Thread.Sleep(2000);
            CombineLog.LogInfo("Task RunTaskForGetFundEstimationJump " + fundId + " started");
            result = _specifyFundJumpService.ExecuteCrawl(fundId);
            CombineLog.LogInfo("Task RunTaskForGetFundEstimationJump " + fundId + " ended");
            return result;
        }

        private string RunTaskForBuyFund(string fundId, double investAmount)
        {
            string result = "";
            Thread.Sleep(2000);
            if (investAmount > 0)
            {
                CombineLog.LogInfo("Task RunTaskForBuyFund " + fundId + " " + investAmount + " started");
                result = _specifyFundBuyService.ExecuteBuy(fundId, investAmount.ToString(CultureInfo.InvariantCulture));
                CombineLog.LogInfo("Task RunTaskForBuyFund " + fundId + " " + investAmount + " ended");
            }
            else
            {
                result = "NotBuy";
                CombineLog.LogInfo("Not buy today for " + fundId);
            }
            return result;
        }

        private string RunTaskForSellFund(string fundId, double sellAmount)
        {
            string result = "";
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
            return result;
        }

        public override void Dispose()
        {
            CombineLog.LogInfo("Start to dispose components for fund " + this._fundId);
            _generalPointService.Dispose();
            _specifyFundJumpService.Dispose();
            _specifyFundBuyService.Dispose();
            _specifyFundSellService.Dispose();
            CombineLog.LogInfo("Components are disposed for fund " + this._fundId);
        }
    }
}