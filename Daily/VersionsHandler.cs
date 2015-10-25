using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Daily
{
    public static class VersionsHandler
    {
        public static List<string> getsuitesVersions(List<List<string>> files)
        {
            if (IosRun(files[0])) return new List<string>{""};

            List<string> suitesVersions = new List<string>();
            foreach (List<string> file in files)
            {
                string temp = file[0];
                temp += ": " + getVersion(file);
                suitesVersions.Add(temp + ReplacePlaceHolders.LINE);
            }
            suitesVersions.Add(ReplacePlaceHolders.LINE);

            return suitesVersions;
        }

        private static bool IosRun(List<string> file)
        {
            return file[0].Contains("iOS") || file[1].Contains("iOS");
        }

        public static string getVersion(List<string> file)
        {
            string toReturn = "";

            if (IosRun(file)) return toReturn;
            
            Regex r = new Regex(@"\[[0-9][0-9]:[0-9][0-9]:[0-9][0-9]\] :	 \[Step 1/2\] ([0-9]\.[0-9]\.[0-9][0-9][0-9]\.[0-9])", RegexOptions.IgnoreCase);
            Match m = null;
            foreach (string line in file)
            {
                m = r.Match(line);
                if (m.Success)
                {
                    return m.Groups[1].ToString();
                }
            }

            throw new Exception("App version was not found in suite: " + file[0]);
        }
    }
}