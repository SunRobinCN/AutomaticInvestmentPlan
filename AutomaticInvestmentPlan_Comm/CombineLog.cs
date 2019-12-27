using System;
using System.Diagnostics;

namespace AutomaticInvestmentPlan_Comm
{
    public static class CombineLog
    {
        public static void LogInfo(string message)
        {
            FileLog.Info(message, LogType.Info);
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public static void LogError(string mehtod, Exception ex)
        {
            FileLog.Error(mehtod, ex, LogType.Error);
            Console.WriteLine(mehtod + " " + ex.Message);
            Debug.WriteLine(mehtod + " " + ex.Message);
        }
    }
}