using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Daily
{
    class BuildHandler
    {
        public static string getBuildNumber(List<string> file)
        {
            if (IosRun(file)) return "";

            Regex r = new Regex(@"\[[0-9][0-9]:[0-9][0-9]:[0-9][0-9]\] :	 \[Step 2/2\] Starting Gradle in TeamCity build ([0-9][0-9]?[0-9]?)", RegexOptions.IgnoreCase);
            foreach (string line in file)
            {
                Match m = r.Match(line);
                if (m.Success)
                {
                    return m.Groups[1].ToString();
                }
            }

            throw new Exception("App version was not found in suite: " + file[0]);
        }

        public static List<string> getAllBuildsNumbers(List<List<string>> files)
        {
            if (IosRun(files[0])) return new List<string> { "" };

            List<string> all = new List<string>();
            foreach (List<string> file in files)
            {
                all.Add(getBuildNumber(file));
            }

            return all;
        }

        private static bool IosRun(List<string> file)
        {
            return file[0].Contains("iOS");
        }
    }
}
