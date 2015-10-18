using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Daily
{
    public static class VersionsHandler
    {
        public static List<string> getsuitesVersions(List<List<string>> files)
        {
            List<string> suitesVersions = new List<string>();
            foreach (List<string> file in files)
            {
                string temp = file[0];
                temp += ": " + getVersions(file);
                suitesVersions.Add(temp + Environment.NewLine);
            }
            suitesVersions.Add(Environment.NewLine);
            return suitesVersions;
        }

        public static string getVersions(List<string> file)
        {
            string toReturn = "";
            Regex r = new Regex(@"\[[0-9][0-9]:[0-9][0-9]:[0-9][0-9]\] :	 \[Step 1/2\] ([0-9]\.[0-9]\.[0-9][0-9][0-9]\.[0-9])", RegexOptions.IgnoreCase);
            Match m = null;
            foreach (string line in file)
            {
                m = r.Match(line);
                if (m.Success)
                {
                    toReturn = line;
                    break;
                }
            }

            return m.Groups[1].ToString();
        }
    }
}