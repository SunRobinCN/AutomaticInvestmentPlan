using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticInvestmentPlan_Comm
{
    public static class TaskFactoryExtension
    {
        public static Task StartNew2(this TaskFactory taskFactory, Action action)
        {
            Task t = taskFactory.StartNew(action);
            CacheUtil.Tasks.Add(t);
            return t;
        }
    }
}