using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Daily.Build;

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

        public static List<string> getAllBuildsNumbers(List<TcBuild> builds)
        {
            return builds.Select(build => getBuildNumber(build.Log[0])).ToList();
        }

        public static string getBuildAsLink(string buildName, List<string> fileLines)
        {
            string link = fileLines[3].Replace("TeamCity URL ", "") + "&tab=artifacts";
            return new LinkCreator().makeLink(buildName, link);
        }
    }
}
