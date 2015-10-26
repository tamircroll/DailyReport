using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    public class FilesHandler
    {
        const string logsFolder = "c:/DailyReport/";
        public List<List<string>> GetAllAndroidFiles()
        {
            var list = new List<List<string>>
            {
                GetTestsList("TechnicianView"),
                GetTestsList("FirstExperience"),
                GetTestsList("OngoingValue"),
                GetTestsList("TechExpertExperience")
            };
            list.RemoveAll(item => item == null);
            getAndUpdateArtifactsLink(list);
            return list;
        }

        private void getAndUpdateArtifactsLink(List<List<string>> list)
        {
            foreach (var suite in list)
            {
                var url = suite[4].Replace("TeamCity URL ","") + "&tab=artifacts";
                suite[0] = new LinkCreator().makeLink(suite[0], url);
            }
        }

        private List<string> GetTestsList(string suite)
        {
            var builder = new StringBuilder();
            foreach (var c in suite)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            var suiteSplittedByCapitel = builder.ToString();

            try
            {
                return new List<string>(
                        new List<string> { suite }
                            .Concat(File.ReadAllLines(getLatestLogPath(logsFolder, suiteSplittedByCapitel), Encoding.UTF8)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<List<string>> getAllFilesFromDirectory(string folderPath, string desclude)
        {
            List<List<string>> toReturn = new List<List<string>>();
            List<string> filesPaths = Directory.GetFiles(folderPath).ToList();

            foreach (string path in filesPaths)
            {
                if (!path.Contains(desclude))
                {
                    toReturn.Add(new List<string>(File.ReadAllLines(path, Encoding.UTF8)));
                }
            }

            return toReturn;
        }

        public static string setErrorAndTestName(string error, string test)
        {
            return error + " " + test;
        }

        public static string getNameByBuilds(List<string> builds)
        {
            string name = "";
            foreach (string build in builds)
            {
                name += build + "_";
            }

            return name.Remove(name.Length - 1) + ".txt";
        }

        private string getLatestLogPath(string folder, string suiteName)
        {
            var pattern = suiteName.Split(' ').Aggregate("", (current, s) => current + (".*" + s)) + @".*?(\d+)";
            var regex = new Regex(pattern);
            var toReturn = "";
            var maxBuildNumber = -1;

            foreach (var file in Directory.GetFiles(folder))
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