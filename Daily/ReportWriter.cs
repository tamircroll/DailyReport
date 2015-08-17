using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    internal class ReportWriter
    {
        private const int FAILED = 0;
        private const int SUCCESS = 1;
        private const int IGNORED = 2;

        private List<string> output = new List<string>();

        public void Write(string msg)
        {
            string body = msg
                .Replace(MessageBuilder.START_PARAGRAPH, "")
                .Replace(MessageBuilder.CLOSE_PARAGRAPH, "")
                .Replace(MessageBuilder.SPAN_SMALL, "   ")
                .Replace(MessageBuilder.SPAN_RED, "")
                .Replace(MessageBuilder.SPAN_GREEN, "")
                .Replace(MessageBuilder.CLOSE_SPAN, "")
                .Replace(MessageBuilder.LINE, "\n");
            output.Add(body);
            File.WriteAllLines("c:/DailyReport/output.txt", output);
        }
    }
}
