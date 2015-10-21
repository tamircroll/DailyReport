using System;
using System.Net;
using System.Net.Mail;

namespace Daily
{
    internal class MailSender
    {
        public void SendMail(string msg, string version)
        {
            var client = new SmtpClient("smtp");
            client.UseDefaultCredentials = false;
            var basicAuthenticationInfo = new NetworkCredential("soluto.local\tamir", "Qwer1234");
            client.Credentials = basicAuthenticationInfo;

            var from = new MailAddress("tamir@soluto.com", "Tamir: ");
            var to = new MailAddress("tamir@soluto.com", "TestToName");
            var myMail = new MailMessage(from, to)
            {
                Subject = "Automation Tests Status - " + DateTime.Now.ToString("dd/MM/yyy") + ", Version: " + version,
                SubjectEncoding = System.Text.Encoding.UTF8,
                Body = msg,
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true
            };
            try
            {
                client.Send(myMail);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                    ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
