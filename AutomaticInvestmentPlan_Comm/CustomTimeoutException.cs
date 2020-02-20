using System;

namespace AutomaticInvestmentPlan_Host
{
    public class CustomTimeoutException : Exception
    {
        public CustomTimeoutException(string message) : base(message)
        {
           
        }
    }
}