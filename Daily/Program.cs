using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

//Output HTML using ctrl+u - and before to open all all fields

namespace Daily
{
    internal class Program
    {
        private static void Main()
        {
            string msg = new ReportBuilder().Build();
//            new MailSender().RetryIfBusy(msg);
            new MailSender().CreateTestMessage2(msg);
        }
    }
}