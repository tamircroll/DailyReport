using System;
using System.Net;
using System.Net.Mail;

namespace Daily
{
    internal class TestsAnalyzer
    {
        string server = "my.smtp.exampleserver.net";

        public void CreateTestMessage2()
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("my.smtp.exampleserver.net");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                    System.Net.NetworkCredential("username", "password");
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("tamir@soluto.com", "TestFromName");
                MailAddress to = new MailAddress("tamir@soluto.com", "TestToName");
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyto = new MailAddress("tamir@soluto.com");
                myMail.ReplyTo = replyto;

                // set subject and encoding
                myMail.Subject = "Test message";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "<b>Test Mail</b><br>using <b>HTML</b>.";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

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

        public void RetryIfBusy(string server)
        {
            MailAddress from = new MailAddress("tamir@soluto.com");
            MailAddress to = new MailAddress("tamir@soluto.com");
            MailMessage message = new MailMessage(from, to);
            // message.Subject = "Using the SmtpClient class.";
            message.Subject = "Using the SmtpClient class.";
            message.Body = @"Using this feature, you can send an e-mail message from an application very easily.";
            // Add a carbon copy recipient.
            MailAddress copy = new MailAddress("Notifications@contoso.com");
            message.CC.Add(copy);
            SmtpClient client = new SmtpClient(server);
            // Include credentials if the server requires them.
            client.Credentials = (ICredentialsByHost)CredentialCache.DefaultNetworkCredentials;
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
