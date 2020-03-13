using System;
using AutomaticInvestmentPlan_Comm;
using Topshelf;

namespace AutomaticInvestmentPlan_Host
{
    class Program
    {
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            try
            {
                // Here is TopShelf to help create windows service and debug it,
                // refer to http://topshelf-project.com/
                HostFactory.Run(serviceConfig =>
                {
                    serviceConfig.Service<ServiceManager>(serviceInstance =>
                    {
                        serviceInstance.ConstructUsing(() => new ServiceManager());
                        serviceInstance.WhenStarted(execute => execute.Start());
                        serviceInstance.WhenStopped((execute, hostControl) => execute.Stop(hostControl));
                    });
                    serviceConfig.SetServiceName("HCC.APP.INVESTMENT.CALCULATOR.HOST");
                    serviceConfig.SetDescription("This is the windows service to host the HCC.APP.INVESTMENT.CALCULATOR.HOST");
                    serviceConfig.SetDisplayName("HCC.APP.INVESTMENT.CALCULATOR.HOST");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                FileLog.Error("Program.Main", ex, LogType.Error);
                Console.ReadLine();
            };
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is CustomTimeoutException customTimeoutException)
            {
                CombineLog.LogError("UnhandledExceptionTrapper " + customTimeoutException.Info, customTimeoutException);
            }
            else
            {
                CombineLog.LogError("UnhandledExceptionTrapper ", (Exception)e.ExceptionObject);
            }
        }
    }
}
