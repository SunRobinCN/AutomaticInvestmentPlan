using System;

namespace AutomaticInvestmentPlan_Model
{
    public class MyDisposable
    {
        public bool JobDone = false;
        public string Name;

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}