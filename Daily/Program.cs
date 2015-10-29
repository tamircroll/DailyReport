namespace Daily
{
    internal static class Program
    {
        
        private static void Main()
        {
//            new PrintAllTestsNames().print();
            var allFiles = new BuildsFromFilesRetriver().Get();
            var msg = new MessageBuilder(allFiles);
            new FileWriter().Write(msg.ReplacePlaceHolders.GetTextMessage());
            new FileWriter().Write(msg.TestsHandler.FailedTests,msg.TestsHandler.Builds);
            new MailSender().SendMail(msg.ReplacePlaceHolders.GetHtmlMessage(), msg.TestsHandler.Versions);
        }
    }
}