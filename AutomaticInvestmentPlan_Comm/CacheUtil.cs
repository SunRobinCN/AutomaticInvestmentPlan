using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticInvestmentPlan_Comm
{
    public static class CacheUtil
    {
        public static double GeneralPoint;

        public static List<double> SpecifyPointJumpHistory;

        public static double SpecifyEstimationJumpPoint;

        public static string Name;

        public static string BuyAmount;

        public static string BuyResult;

        public static List<Task> Tasks = new List<Task>();

        public static void RefrshCache()
        {
            GeneralPoint = 0;
            SpecifyPointJumpHistory = null;
            SpecifyEstimationJumpPoint = 0;
            Name = "";
            BuyAmount = "";
            BuyResult = "";
            Tasks = new List<Task>();
        }
    }
}