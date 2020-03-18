using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_Host
{
    public class CombinedResult
    {
        public GeneralPointModel GeneralPoint { get; set; }
        public string FundName { get; set; }
        public double EstimationJumpPercentage { get; set; }
        public double EstimationValue { get; set; }
        public double InvestAmount { get; set; }
    }
}
