using System.Runtime.ExceptionServices;

namespace Daily
{
    internal static class Program
    {
        private static void Main()
        {
//            new PrintAllTestsNames().print();
            var msg = new MessageBuilder(new FilesHandler().getAllAndroidFiles());
            new FileWriter().Write(msg.ReplacePlaceHolders.GetTextMessage());
            new FileWriter().Write(msg.TestsHandler.FailedTests, msg.Builds);
            new MailSender().SendMail(msg.ReplacePlaceHolders.GetHtmlMessage(), msg.SomeVersion);
        }
    }
}