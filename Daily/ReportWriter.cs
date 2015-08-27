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
        private readonly List<string> _output = new List<string>();

        public void Write(string msg)
        {
            _output.Add(msg);
            File.WriteAllLines("c:/DailyReport/output.txt", _output);
        }
    }
}
