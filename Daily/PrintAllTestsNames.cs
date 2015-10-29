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
            var allTests = new List<string>();
            var builds = new BuildsFromFilesRetriver().Get();

            foreach (var build in builds)
                allTests.AddRange(from line in build.Log where line.StartsWith(" Test name: ") select line.Replace(" Test name: ", ""));
             File.WriteAllLines("c:/DailyReport/allTests.txt", allTests);
        }
    }
}