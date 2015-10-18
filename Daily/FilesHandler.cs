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
    }
}