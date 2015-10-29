using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Daily
{
    internal static class Program
    {
        private static void Main()
        {
//            new PrintAllTestsNames().print();
            List<List<string>> allFiles = new FilesHandler().GetAllAndroidFiles();
            var msg = new MessageBuilder(allFiles);
            new FileWriter().Write(msg.ReplacePlaceHolders.GetTextMessage());
            new FileWriter().Write(msg.TestsHandler.FailedTests,msg.TestsHandler.Builds);
            new MailSender().SendMail(msg.ReplacePlaceHolders.GetHtmlMessage(), msg.TestsHandler.Versions);
        }
    }
}