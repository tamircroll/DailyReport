using System;
using System.Net;
using System.Net.Mail;

namespace Daily
{
    internal class MailSender
    {
        public void CreateTestMessage2(string msg)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                var basicAuthenticationInfo = new
                    NetworkCredential("soluto.local\tamir", "Qwer1234");
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                var from = new MailAddress("tamir@soluto.com", "Tamir: ");
                var to = new MailAddress("tamir@soluto.com", "TestToName");
                var myMail = new System.Net.Mail.MailMessage(from, to)
                {
                    Subject = "Temp daily report. Date: " + DateTime.Now.ToString("dd/MM/yyy"),
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    Body = msg
                        .Replace(MessageBuilder.START_PARAGRAPH, "<p>")
                        .Replace(MessageBuilder.CLOSE_PARAGRAPH, "</p>")
                        .Replace(MessageBuilder.SPAN_SMALL, "<span style='font-size: 10pt'>&nbsp&nbsp&nbsp")
                        .Replace(MessageBuilder.SPAN_RED, "<span style = 'color:red'>")
                        .Replace(MessageBuilder.SPAN_GREEN, "<span style = 'color:green'>")
                        .Replace(MessageBuilder.CLOSE_SPAN, "</span>")
                        .Replace(MessageBuilder.LINE, "<br>"),
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true
                };

                mySmtpClient.Send(myMail);
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



        public void RetryIfBusy(string msg)
        {
            MailAddress from = new MailAddress("tamir@soluto.com");
            MailAddress to = new MailAddress("tamir@soluto.com");
            MailMessage message = new MailMessage(from, to);
            // message.Subject = "Using the SmtpClient class.";
            message.Subject = "Using the SmtpClient class.";
            message.Body = msg.Replace("{0}", "<span style='font-size: 10pt'>&nbsp&nbsp&nbsp")
                .Replace(MessageBuilder.CLOSE_SPAN, "</span>")
                        .Replace(MessageBuilder.CLOSE_PARAGRAPH, "</p>")
                        .Replace(MessageBuilder.SPAN_RED, "<span style = 'color:red'>")
                        .Replace(MessageBuilder.SPAN_GREEN, "<span style = 'color:green'>")
                        .Replace(MessageBuilder.LINE, "<br>")
                        .Replace(MessageBuilder.START_PARAGRAPH, "<p>");
            message.IsBodyHtml = true;
            // Add a carbon copy recipient.
            // MailAddress copy = new MailAddress("tamir@soluto.com");
            // message.CC.Add(copy);
            SmtpClient client = new SmtpClient("smtp");
            // Include credentials if the server requires them.
            client.Credentials = (ICredentialsByHost) CredentialCache.DefaultNetworkCredentials;
            Console.WriteLine("Sending an e-mail message to {0} using the SMTP host {1}.",
                to.Address, client.Host);
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
                Console.WriteLine("Exception caught in RetryIfBusy(): {0}",
                    ex.ToString());
            }
        }
    }
}
