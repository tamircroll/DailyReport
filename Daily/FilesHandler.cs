using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daily
{
    public class FilesHandler
    {
        public List<List<string>> getAllAndroidFiles()
        {
            var files = new List<List<string>>
            {
                new List<string>(
                    new List<string> {"TechnicianView"}
                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_Technician_View.log", Encoding.UTF8))),
                new List<string>(
                    new List<string> {"FirstExperience"}
                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_First_Experience.log",
                            Encoding.UTF8))),
                new List<string>(
                    new List<string> {"OngoingValue"}
                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_Ongoing_Value.log", Encoding.UTF8))),
                new List<string>(
                    new List<string> {"TechExpertExperienceTests"}
                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_Tech_Expert_Experience.log",
                            Encoding.UTF8)))
            };

            return files;
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
    }
}