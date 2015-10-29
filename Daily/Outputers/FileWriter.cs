using System;
using System.Collections.Generic;
using System.IO;

namespace Daily
{
    internal class FileWriter
    {
        private readonly List<string> _output = new List<string>();


        public void Write(string msg, string path = "c:/DailyReport/output.txt")
        {
            _output.Add(msg);
            File.WriteAllLines(path, _output);
        }

        public void Write(SortedDictionary<string, List<Test>> failedTests, List<string> builds)
        {
            string msg = "";
            foreach (string error in failedTests.Keys)
            {
                List<Test> tests;
                failedTests.TryGetValue(error, out tests);
                foreach (Test test in tests)
                {
                    msg += BuildsFromFilesRetriver.setErrorAndTestName(error, test.ToString()) + Environment.NewLine;
                }
            }

            string fileName = BuildsFromFilesRetriver.getNameByBuilds(builds);

            Write(msg, "c:/DailyReport/OldReports/" + fileName);
        }
    }
}
