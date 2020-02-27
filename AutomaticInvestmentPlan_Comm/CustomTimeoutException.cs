using System;

namespace AutomaticInvestmentPlan_Comm
{
    public class CustomTimeoutException : Exception
    {
        public string Info { get; set; }

        public CustomTimeoutException(string message) : base(message)
        {
           
        }
    }
}