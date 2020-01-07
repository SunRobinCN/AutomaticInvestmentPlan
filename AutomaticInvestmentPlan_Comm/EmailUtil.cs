using System;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace AutomaticInvestmentPlan_Comm
{
    public static class EmailUtil
    {
        public static void Send(string subject, string body)
        {
            MailMessage mailMessage =
                new MailMessage {From = new MailAddress("Investment.Monitor@specialmail.homecreditcfc.cn")};
            mailMessage.To.Add("robin.sun@homecreditcfc.cn");  
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            try
            {
                SmtpClient smtpClient = new SmtpClient
                {
                    Host = "relay.Homecredit.cn",
                    Port = 25
                };
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                FileLog.Error("EmailHelper.Send1" , ex, LogType.Error);
                Thread.Sleep(1000*2);
                try
                {
                    SmtpClient smtpClient = new SmtpClient
                    {
                        Host = "relay.Homecredit.cn",
                        Port = 25
                    };
                    smtpClient.Send(mailMessage);
                }
                catch (Exception e)
                {
                    FileLog.Error("EmailHelper.Send2", e, LogType.Error);
                }
            }
        }
    }
}
