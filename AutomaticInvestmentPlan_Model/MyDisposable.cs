using System;

namespace AutomaticInvestmentPlan_Model
{
    public class MyDisposable : IDisposable
    {
        public bool JobDone = false;

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}