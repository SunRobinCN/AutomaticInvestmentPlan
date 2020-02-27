using System;

namespace AutomaticInvestmentPlan_Model
{
    public class MyDisposable : IDisposable
    {
        public bool JobDone = false;
        public string Name;

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}