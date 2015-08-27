namespace Daily
{
    internal class Program
    {
        private static void Main()
        {
            var msg = new MessageBuilder();
            new ReportWriter().Write(msg.GetTextMessage());
            new MailSender().SendMail(msg.GetHtmlMessage());
        }
    }
}