using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutomaticInvestmentPlan_LogViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];
            int cachedLength = 0;
            while (true)
            {
                if (File.Exists(logPath) == false)
                {
                   continue;
                }
                string content = File.ReadAllText(logPath, Encoding.UTF8);
                string showContent = content.Substring(cachedLength, content.Length-cachedLength);
                if (content.Length - cachedLength > 0)
                {
                    cachedLength = content.Length;
                    string[] list = showContent.Replace("\r\n", "@").Split('@');
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (i == 0)
                        {
                            Console.Write(list[i]);
                        }
                        else
                        {
                            Console.WriteLine(list[i]);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
