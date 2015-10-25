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
        const string folder = "c:/DailyReport/";

        public List<List<string>> getAllAndroidFiles()
        {
            var files = new List<List<string>>();

            tryAddFileToList("TechnicianView", files);
            tryAddFileToList("FirstExperience", files);
            tryAddFileToList("OngoingValue", files);
            tryAddFileToList("TechExpertExperienceTests", files);

            return files;
        }

        private void tryAddFileToList(string fileName, List<List<string>> files)
        {
            try
            {
                List<string> file = new List<string>(
                    new List<string> {splitCapitales(fileName)}
                        .Concat(File.ReadAllLines(getLatestLogPath(folder, "Technician View"), Encoding.UTF8)));
                files.Add(file);
            }
            catch {}
        }

        private string splitCapitales(string fileName)
        {
            var builder = new StringBuilder();
            foreach (var c in fileName)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
         
            return builder.ToString();
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
//                DateTime.Now.Year + "." +
//                DateTime.Now.Month + "." +
//                DateTime.Now.Day + " ";
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