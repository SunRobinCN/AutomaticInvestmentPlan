using System;
using System.Collections.Generic;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_Model
{
    public static class NetworkValueConverter
    {
        public static GeneralPointModel ConvertToGeneralPointModel(string s)
        {
            GeneralPointModel m = new GeneralPointModel();
            string[] arr = s.Split(':');
            if (arr.Length == 2)
            {
                m.Point = arr[0];
                m.Percentate = arr[1];
            }
            return m;
        }

        public static List<HistoryPointModel> ConvertToHistoryPointModel(string s)
        {
            List<HistoryPointModel> list = new List<HistoryPointModel>();
            string raw = s.Substring(3, s.Length - 3);
            string[] arr = raw.Split('%');
            for (int i = 1; i < arr.Length && i<9; i++)
            {
                string date = DateTime.Now.AddDays(-1 * i).ToString("yyyy-MM-dd");
                string point = arr[i-1];
                HistoryPointModel historyPointModel = new HistoryPointModel();
                historyPointModel.Date = date;
                historyPointModel.PointJumpPercentage = point + "%";
                list.Add(historyPointModel);
            }
            return list;
        }

        public static List<HistoryPointModel> ConvertToHistoryPointModel(string dates, string historyJumpPercenage, string historyPointValue)
        {
            List<HistoryPointModel> list = new List<HistoryPointModel>();
            string[] dateArray = dates.Split('^');
            string[] jumpArray = historyJumpPercenage.Split('^');
            string[] pointArray = historyPointValue.Split('^');
            for (int i = 1; i < dateArray.Length && i < 9; i++)
            {
                string date = dateArray[i].Trim();
                string pointJump = jumpArray[i].Trim();
                string pointValue = pointArray[i].Trim();
                HistoryPointModel historyPointModel = new HistoryPointModel
                {
                    Date = date,
                    PointJumpPercentage = pointJump,
                    PointValue = Convert.ToDouble(pointValue)
                };
                list.Add(historyPointModel);
            }
            return list;
        }
    }


}

//日涨幅+0.14%-1.41%-0.19%-0.03%-0.11%+0.50%+0.28%+2.15%+0.36%-0.74%-0.20%+0.42%+0.85%+0.20%