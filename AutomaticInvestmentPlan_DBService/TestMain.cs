using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_DBService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            DbService dbService = new DbService();
            HistoryModel m = new HistoryModel();
            m.BuyDate = DateTime.Now;
            m.FundValue = 1.55;
            m.FundValueJumpPercentage = 0.09;
            m.BuyAmount = 150;
            m.AlreaySold = 0;
            dbService.InsertBuyResult(m);
            List<HistoryModel> list = dbService.SelectAllNotSold("");
            HistoryModel model = list[0];
            model.AlreaySold = 1;
            model.SellDate = DateTime.Now;
            model.FundValueInSell = 1.8;
            model.SellAmount = 200;
            model.Profit = 20;
            model.ProfitPercentage = 0.2;
            //dbService./*Update*/(model);
            list = dbService.SelectAllNotSold("");
        }
    }
}
