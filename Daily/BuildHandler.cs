﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Daily
{
    static class BuildHandler
    {
        public static string getBuildNumber(string line)
        {
            Regex r = new Regex(@".*#([0-9][0-9]?[0-9]?)", RegexOptions.IgnoreCase);

            Match m = r.Match(line);
            if (m.Success) return m.Groups[1].ToString();
            throw new Exception("App version was not found in suite: " + line);
        }

        public static List<string> getAllBuildsNumbers(List<List<string>> files)
        {
            List<string> all = new List<string>();
            foreach (List<string> file in files)
            {
                all.Add(getBuildNumber(file[1]));
            }

            return all;
        }

        public static string getBuildAsLink(string buildName, List<string> fileLines)
        {
            string link = fileLines[4].Replace("TeamCity URL ", "") + "&tab=artifacts";
            return new LinkCreator().makeLink(buildName, link);
        }

        public static string getBuildAsLink(List<string> fileLines)
        {
            return getBuildAsLink(fileLines[0], fileLines);
        }
    }
}
