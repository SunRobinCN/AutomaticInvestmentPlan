using System.Collections.Generic;

namespace AutomaticInvestmentPlan_Comm
{
    public static class CalculateUtil
    {
        public static double CalculateInvestmentAmount(string fundId, double genralPoint,
            double estimatedJump,
            List<double> jumpHistory)
        {
            if (CacheUtil.CheckWhetherStrategyFund(fundId) == false)
            {
                return CalculateInvestmentAmountForNormal(genralPoint, estimatedJump, jumpHistory);
            }
            return CalculateInvestmentAmountForUpgradStrategy(genralPoint, estimatedJump, jumpHistory);
        }


        public static double CalculateInvestmentAmountForNormal(double genralPoint, double estimatedJump, List<double> jumpHistory)
        {
            int _baseGeneralPoint = 3500;
            int _baseAmount = 30;
            int _jumpBaseAmount = 3000;
            if (estimatedJump > 0)
            {
                return 0;
            }
            //距离_baseGeneralPoint点每点0.1元
            double acculatedAmount1 = 0.1 * (genralPoint - _baseGeneralPoint) * -1;
            double acculatedAmount2 = 0;
            double amountForDay0 = _jumpBaseAmount * estimatedJump * -1;
            double jumpRateD1 = jumpHistory[0];
            double jumpRateD2 = jumpHistory[1];
            double jumpRateD3 = jumpHistory[2];
            if (jumpRateD1 > 0)
            {
                acculatedAmount2 = amountForDay0;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }

            double amountForDay1 = _jumpBaseAmount * jumpRateD1 * -1;
            if (jumpRateD2 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1) * 2;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay2 = _jumpBaseAmount * jumpRateD2 * -1;
            if (jumpRateD3 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2) * 3;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay3 = _jumpBaseAmount * jumpRateD3 * -1;
            acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2 + amountForDay3) * 4;
            return _baseAmount + acculatedAmount1 + acculatedAmount2;
        }


        public static double CalculateInvestmentAmountForUpgradStrategy(double genralPoint, double estimatedJump,
            List<double> jumpHistory)
        {
            int _baseGeneralPoint = 3500;
            int _baseAmount = 88;
            int _jumpBaseAmount = 3000;
            if (estimatedJump > 0.005)
            {
                return 0;
            }
            //距离_baseGeneralPoint点每点0.1元
            double acculatedAmount1 = 0.1 * (genralPoint - _baseGeneralPoint) * -1;
            double acculatedAmount2 = 0;
            double amountForDay0 = _jumpBaseAmount * estimatedJump * -1;
            double jumpRateD1 = jumpHistory[0];
            double jumpRateD2 = jumpHistory[1];
            double jumpRateD3 = jumpHistory[2];
            if (jumpRateD1 > 0)
            {
                acculatedAmount2 = amountForDay0;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }

            double amountForDay1 = _jumpBaseAmount * jumpRateD1 * -1;
            if (jumpRateD2 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1) * 2;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay2 = _jumpBaseAmount * jumpRateD2 * -1;
            if (jumpRateD3 > 0)
            {
                acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2) * 3;
                return _baseAmount + acculatedAmount1 + acculatedAmount2;
            }
            double amountForDay3 = _jumpBaseAmount * jumpRateD3 * -1;
            acculatedAmount2 = (amountForDay0 + amountForDay1 + amountForDay2 + amountForDay3) * 4;
            return _baseAmount + acculatedAmount1 + acculatedAmount2;
        }

    }
}