using System;

namespace AutomaticInvestmentPlan_Model
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public DateTime BuyDate { get; set; }
        public string FundId { get; set; }
        public string FundName  { get; set; }
        public double ShangHaiIndex { get; set; }
        public string ShangHaiIndexJumpPercentage { get; set; }
        public double FundValue { get; set; }
        public double FundValueJumpPercentage { get; set; }
        public double BuyAmount { get; set; }
        public int AlreaySold { get; set; }
        public DateTime ?SellDate { get; set; }
        public double FundValueInSell { get; set; }
        public double SellAmount { get; set; }
        public double Profit { get; set; }
        public double ProfitPercentage { get; set; }
        public double FundShare { get; set; }
    }
}