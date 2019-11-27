using System;
using System.Net.Mail;
using System.Text;

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
            SmtpClient smtpClient = new SmtpClient();
            try
            {
                smtpClient.Host = "relay.Homecredit.cn";
                smtpClient.Port = 25;
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                FileLog.Error("EmailHelper.Send" , ex, LogType.Error);
            }
        }
    }
}
