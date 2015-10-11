namespace Daily
{
    internal class Program
    {
        private static void Main()
        {
            var msg = new MessageBuilder();
            new ReportWriter().Write(msg.ReplacePlaceHolders.GetTextMessage());
            new MailSender().SendMail(msg.ReplacePlaceHolders.GetHtmlMessage());
        }
    }
}