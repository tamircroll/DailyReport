using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Daily
{
    class PrintAllTestsNames
    {
        public void print()
        {
            List<string> allTests = new List<string>();
            var files = new FilesHandler().GetAllAndroidFiles();

            foreach (var file in files)
                allTests.AddRange(from line in file where line.StartsWith(" Test name: ") select line.Replace(" Test name: ", ""));
             File.WriteAllLines("c:/DailyReport/allTests.txt", allTests);
        }
    }
}