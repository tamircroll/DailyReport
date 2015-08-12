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

        List<string> output = new List<string>();

        public void Write(string msg)
        {
            string body = msg.Replace("{0}", "   ").Replace("{1}", "").Replace("{2}", "").Replace("{3}", "").Replace("{4}", "").Replace("{5}", "\n").Replace("{6}", "");
            output.Add(body);
            File.WriteAllLines("c:/DailyReport/output.txt", output);
        }
    }
}
