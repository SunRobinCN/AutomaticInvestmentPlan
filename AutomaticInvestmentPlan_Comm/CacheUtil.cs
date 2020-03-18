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
        public List<double> AcculatedPointHistory;
        public double EstimationFundValue;
        public double EstimationJumpPercentage;
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
        public static List<string> EftList = new List<string>()
        {
            "515030"
        };

        public static double MinIncreasePercentage = 0.1;
        public static double MaxIncreasePercentage = 0.3;

        public static string BuyAmount;

        private static readonly Dictionary<string, string> FundNameDictionary = new Dictionary<string, string>();
        private static List<SpecifyFundCache> _specifyFundCacheList = new List<SpecifyFundCache>();
        private static readonly Dictionary<string, GeneralPointCache> GeneralPointCacheList = new Dictionary<string, GeneralPointCache>();
        private static readonly List<string> StrategyFunds = new List<string>();

        public static void AddStrategyFund(string fundId)
        {
            if (StrategyFunds.Contains(fundId) == false)
            {
                StrategyFunds.Add(fundId);
            }
        }

        public static bool CheckWhetherStrategyFund(string fundId)
        {
            return StrategyFunds.Contains(fundId);
        }

        public static void RefrshCache()
        {
            BuyAmount = "0";
            _specifyFundCacheList = new List<SpecifyFundCache>();
        }

        public static void AddFundNameInCache(string fundId, string fundName)
        {
            if (FundNameDictionary.ContainsKey(fundId) == false)
            {
                FundNameDictionary.Add(fundId, fundName);
            }
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
                SpecifyFundCache specifyFundCache = new SpecifyFundCache {FundId = fundId};
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