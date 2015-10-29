using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Daily.Build;

namespace Daily
{
    public static class VersionsHandler
    {
        public static List<string> getsuitesVersions(List<TcBuild> builds)
        {
            TcBuild tcBuild = builds.First();
            if (IosRun(tcBuild.Link, tcBuild.Log)) return new List<string> { "" };

            var suitesVersions = new List<string>();
            foreach (var build in builds)
            {
                string temp = build.Link;
                temp += ": " + getVersion(build.Link, build.Log);
                suitesVersions.Add(temp + ReplacePlaceHolders.LINE);
            }
            suitesVersions.Add(ReplacePlaceHolders.LINE);

            return suitesVersions;
        }

        private static bool IosRun(string link, List<string> file)
        {
            return link.Contains("iOS") || file[0].Contains("iOS");
        }

        public static string getVersion(string link, List<string> file)
        {
            string toReturn = "";

            if (IosRun(link, file)) return toReturn;
            
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

        public static List<string> getVersions(List<TcBuild> builds)
        {
            var versions = new List<string>();
            foreach (var build in builds)
            {
                string version = getVersion(build.Link, build.Log);
                if (versions.Any(s => s.Contains(version))) continue;
                versions.Add(version);
            }

            return versions;
        }

        public static string getVersionsStr(List<TcBuild> builds)
        {
            List<string> versions = getVersions(builds);
            String versionsStr = versions.Aggregate("", (current, version) => current + version + "/");

            return versionsStr.Remove(versionsStr.Length - 1);
        }
    }
}