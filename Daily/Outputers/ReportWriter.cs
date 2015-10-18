using System.Collections.Generic;
using System.IO;

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
