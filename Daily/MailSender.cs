using System;
using System.Net;
using System.Net.Mail;

namespace Daily
{
    internal class MailSender
    {
        public void SendMail(string msg)
        {

            SmtpClient client = new SmtpClient("smtp");

            // set smtp-client with basicAuthentication
            client.UseDefaultCredentials = false;
            var basicAuthenticationInfo = new NetworkCredential("soluto.local\tamir", "Qwer1234");
            client.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            var from = new MailAddress("tamir@soluto.com", "Tamir: ");
            var to = new MailAddress("tamir@soluto.com", "TestToName");
            var myMail = new MailMessage(from, to)
            {
                Subject =
                    "Automation Tests Status - " + DateTime.Now.ToString("dd/MM/yyy") + ", Version: !!!!!!TEMP!!!!!",
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


        public void SendMailWithRetry(string msg)
        {
            var from = new MailAddress("tamir@soluto.com");
            var to = new MailAddress("tamir@soluto.com");
            var message = new MailMessage(from, to);
            message.Subject = "Automation Tests Status - " + DateTime.Now.ToString("dd/MM/yyy") +
                              ", Version: !!!!!!TEMP!!!!!";
            message.Body = msg;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp");
            // Include credentials if the server requires them.
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            Console.WriteLine("Sending an e-mail message to {0} using the SMTP host {1}.", to.Address, client.Host);
            try
            {
                client.Send(message);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        client.Send(message);
                    }
                    else
                    {
                        Console.WriteLine("Failed to deliver message to {0}",
                            ex.InnerExceptions[i].FailedRecipient);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in SendMailWithRetry(): {0}",
                    ex.ToString());
            }
        }
    }
}
