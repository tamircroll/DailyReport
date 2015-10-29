using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daily.Io
{
    public class LatestLogPathFiner
    {
        private const string logsFolder = "c:/DailyReport/";
        public string Find(string suiteName)
        {
            var pattern = suiteName.Split(' ').Aggregate("", (current, s) => current + (".*" + s)) + @".*?(\d+)";
            var regex = new Regex(pattern);
            var toReturn = "";
            var maxBuildNumber = -1;

            foreach (var file in Directory.GetFiles(logsFolder))
            {
                if (regex.IsMatch(file))
                {
                    var buildNumber = Int32.Parse(regex.Match(file).Groups[1].Value);
                    if (buildNumber > maxBuildNumber)
                    {
                        maxBuildNumber = buildNumber;
                        toReturn = file;
                    }
                }
            }

            if (toReturn == "")
                throw new FileNotFoundException();
            return toReturn;
        }
    }
}
