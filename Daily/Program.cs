namespace Daily
{
    internal class Program
    {
        private static void Main()
        {
            string msg = new MessageBuilder().Build();
            new ReportWriter().Write(msg);
            new MailSender().CreateTestMessage2(msg);
        }
    }
}