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
            new ReportBuilder().Build();
            string msg = new MailReportBuilder().Build();
            new MailSender().CreateTestMessage2(msg);
//            new MailSender().RetryIfBusy(msg);
        }
    }
}