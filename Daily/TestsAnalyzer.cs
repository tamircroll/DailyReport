using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daily
{
    internal class TestsAnalyzer
    {
        public void Analize()
        {

            List<List<string>> files = new List<List<string>>();

            files.Add(
                new List<string>(
                    new List<string> {"TechnicianView-RealDevice"}.Concat(
                        File.ReadAllLines("c:/DailyReport/TechnicianView-RealDevice.txt", Encoding.UTF8))));
            files.Add(
                new List<string>(
                    new List<string> {"FirstExperience"}.Concat(File.ReadAllLines("c:/DailyReport/FirstExperience.txt",
                        Encoding.UTF8))));
            files.Add(
                new List<string>(
                    new List<string> {"MultipleDevices"}.Concat(File.ReadAllLines("c:/DailyReport/MultipleDevices.txt",
                        Encoding.UTF8))));
            files.Add(
                new List<string>(
                    new List<string> {"OngoingValue"}.Concat(File.ReadAllLines("c:/DailyReport/OngoingValue.txt",
                        Encoding.UTF8))));
            files.Add(
                new List<string>(
                    new List<string> {"TechExpertExperienceTests"}.Concat(
                        File.ReadAllLines("c:/DailyReport/techExpertExperienceTests.txt", Encoding.UTF8))));

            var output2 = new List<string>();


            File.WriteAllLines("c:/DailyReport/output.txt", output2);
        }
    }
}
