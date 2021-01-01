using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_Comm
{
    public static class DayUtil
    {
        public static bool WhetherWeekend()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SkipWorkingDayCheck"] == "true")
            {
                return false;
            }
            DayOfWeek day = DateTime.Now.DayOfWeek;
            bool result = (day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday);
            return result;
        }

        public static bool WhetherHoliday()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["SkipWorkingDayCheck"] == "true")
            {
                return false;
            }
            bool result;
            try
            {
                string url = "http://timor.tech/api/holiday/info/";

                using (HttpClient client = new HttpClient())
                {
                    Task<string> t = client.GetStringAsync(url);
                    string r = t.Result;
                    HttpResultModel m = HttpResultModel.FromJson(r);
                    result = (m.Type.Type == 1) || (m.Type.Type ==2);
                }
            }
            catch (Exception e)
            {
                CombineLog.LogError("WorkDayService.WhetherWorkDay", e);
                result = false;
            }
            return result;
        }
    }
}
