using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomaticInvestmentPlan_Comm;
using Topshelf;

namespace AutomaticInvestmentPlan_Host
{
    class Program
    {
        static void Main(string[] args)
        {
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

        //static void Main(string[] args)
        //{
        //    Task<int> t = DoSumAsync(1, 2);
        //    Console.WriteLine("结果:{0}", t.Result);
        //    Console.ReadKey();
        //}

        ////2.异步方法
        //public static async Task<int> DoSumAsync(int a, int b)
        //{
        //    //3.await 表达式
        //    Console.WriteLine("a");
        //    Thread.Sleep(2000);
        //    Console.WriteLine("1");
        //    int sum = await Task.Run(() =>
        //    {
        //        Console.WriteLine("f");
        //        return a + b;
        //    });
        //    Console.WriteLine("b");
        //    return sum;
        //}
    }
}
