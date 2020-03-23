using System.Collections.Generic;
using System.Linq;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_Comm
{
    public static class CalculateUtil
    {
        public static double CalculateInvestmentAmount(string fundId, double todayFundValue, double genralPoint,
            double estimatedJump, List<double> jumpHistory, List<double> accumulatedPointHistory)
        {
            if (CacheUtil.CheckWhetherStrategyFund(fundId) == false)
            {
                return CalculateInvestmentAmountForNormal(genralPoint, todayFundValue, estimatedJump, jumpHistory, accumulatedPointHistory) * 0.3;
            }
            return CalculateInvestmentAmountForUpgradStrategy(genralPoint, todayFundValue, estimatedJump, jumpHistory, accumulatedPointHistory) * 0.3;
        }


        public static double CalculateInvestmentAmountForNormal(double genralPoint, double todayFundValue, double estimatedJump, List<double> jumpHistory, List<double> accumulatedPointHistory)
        {
            bool falling = CalculateWhetherFalling(accumulatedPointHistory, todayFundValue);

            int baseGeneralPoint = 3500;
            int baseAmount = 30;
            if (falling)
            {
                baseAmount = baseAmount + 30;
            }
            int jumpBaseAmount = 3000;
            if (estimatedJump > 0.002)
            {
                return 0;
            }
            //距离_baseGeneralPoint点每点0.1元
            double acculatedAmount1 = 0.05 * (genralPoint - baseGeneralPoint) * -1;
            double acculatedAmount2 = 0;
            double amountForDay0 = jumpBaseAmount * estimatedJump * -1;
            double jumpRateD1 = jumpHistory[0];
            double jumpRateD2 = jumpHistory[1];
            double jumpRateD3 = jumpHistory[2];
            if (jumpRateD1 > 0)
            {
                acculatedAmount2 = amountForDay0;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }

            double amountForDay1 = jumpBaseAmount * jumpRateD1 * -1;
            if (jumpRateD2 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1) * 1.2;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay2 = jumpBaseAmount * jumpRateD2 * -1;
            if (jumpRateD3 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2) * 1.5;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay3 = jumpBaseAmount * jumpRateD3 * -1;
            acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2 + amountForDay3) * 2;
            return baseAmount + acculatedAmount1 + acculatedAmount2;
        }


        public static double CalculateInvestmentAmountForUpgradStrategy(double genralPoint, double todayFundValue, double estimatedJump,
            List<double> jumpHistory, List<double> accumulatedPointHistory)
        {
            bool falling = CalculateWhetherFalling(accumulatedPointHistory, todayFundValue);

            int baseGeneralPoint = 3500;
            int baseAmount = 88;
            int jumpBaseAmount = 3000;
            if (falling)
            {
                baseAmount = baseAmount + 30;
            }
            if (estimatedJump > 0.005)
            {
                return 0;
            }
            //距离_baseGeneralPoint点每点0.1元
            double acculatedAmount1 = 0.05 * (genralPoint - baseGeneralPoint) * -1;
            double acculatedAmount2 = 0;
            double amountForDay0 = jumpBaseAmount * estimatedJump * -1;
            double jumpRateD1 = jumpHistory[0];
            double jumpRateD2 = jumpHistory[1];
            double jumpRateD3 = jumpHistory[2];
            if (jumpRateD1 > 0)
            {
                acculatedAmount2 = amountForDay0;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }

            double amountForDay1 = jumpBaseAmount * jumpRateD1 * -1;
            if (jumpRateD2 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1) * 1.2;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay2 = jumpBaseAmount * jumpRateD2 * -1;
            if (jumpRateD3 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2) * 1.5;
                return baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay3 = jumpBaseAmount * jumpRateD3 * -1;
            acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2 + amountForDay3) * 2;
            return baseAmount + acculatedAmount1 + acculatedAmount2;
        }

        public static bool CalculateWhetherSell(HistoryModel historyModel, List<double> accumulatedPointHistory, double todayFundValue)
        {
            if ((todayFundValue - historyModel.FundValue) / historyModel.FundValue >
                CacheUtil.MaxIncreasePercentage)
            {
                return true;
            }

            if (CalculateWhetherFalling(accumulatedPointHistory, todayFundValue))
            {
                if ((todayFundValue - historyModel.FundValue) / historyModel.FundValue >
                    CacheUtil.MinIncreasePercentage)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CalculateWhetherFalling(List<double> accumulatedPointHistory, double todayFundValue)
        {
            double v1 = accumulatedPointHistory[0];
            List<double> subList = accumulatedPointHistory.GetRange(1, accumulatedPointHistory.Count - 1);
            double average1 = subList.Average(d => d);

            double average2 = accumulatedPointHistory.Average(d => d);

            if (v1 < average1 && todayFundValue < average2)
            {
                return true;
            }
            return false;
        }

    }
}