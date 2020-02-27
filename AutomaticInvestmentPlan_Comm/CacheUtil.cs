using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticInvestmentPlan_Comm
{
    public class SpecifyFundCache
    {
        public string FundId;
        public string Name;
        public List<double> SpecifyPointJumpHistory;
        public double EstimationFundValue;
        public double EstimationJumpPoint;
        public string BuyAmount;
        public string BuyResult;
        public string SellShareAmount;
        public string SellResult;
    }

    public class GeneralPointCache
    {
        public double GeneralPoint;
        public string GeneralPointJump;
    }


    public static class CacheUtil
    {
        public static double IncreasePercentage = 0.05;

        public static string BuyAmount;

        private static readonly Dictionary<string, string> FundNameDictionary = new Dictionary<string, string>();
        private static List<SpecifyFundCache> _specifyFundCacheList = new List<SpecifyFundCache>();
        private static readonly Dictionary<string, GeneralPointCache> GeneralPointCacheList = new Dictionary<string, GeneralPointCache>();
        private static readonly List<string> _strategyFunds = new List<string>();

        public static void AddStrategyFund(string fundId)
        {
            if (_strategyFunds.Contains(fundId) == false)
            {
                _strategyFunds.Add(fundId);
            }
        }

        public static bool CheckWhetherStrategyFund(string fundId)
        {
            return _strategyFunds.Contains(fundId);
        }

        public static void RefrshCache()
        {
            BuyAmount = "0";
            _specifyFundCacheList = new List<SpecifyFundCache>();
        }

        public static void AddFundNameInCache(string fundId, string fundName)
        {
            FundNameDictionary.Add(fundId, fundName);
        }

        public static string GetFundNameInCache(string fundId)
        {
            FundNameDictionary.TryGetValue(fundId, out var result);
            return result;
        }

        public static List<SpecifyFundCache> GetAllCaches()
        {
            return _specifyFundCacheList;
        }

        public static SpecifyFundCache GetFundDetailInCache(string fundId)
        {
            List<SpecifyFundCache> list = _specifyFundCacheList.FindAll(s => s.FundId == fundId).ToList();
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                SpecifyFundCache specifyFundCache = new SpecifyFundCache();
                specifyFundCache.FundId = fundId;
                _specifyFundCacheList.Add(specifyFundCache);
                return specifyFundCache;
            }
        }

        public static GeneralPointCache GetGeneralPointInCache(string date)
        {
            if (GeneralPointCacheList.TryGetValue(date, out var generalPointCache) == false)
            {
                GeneralPointCache g = new GeneralPointCache();
                GeneralPointCacheList.Add(date, g);
                return g;
            }
            return generalPointCache;
        }


    }
}