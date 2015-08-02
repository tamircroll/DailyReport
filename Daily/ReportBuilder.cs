using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    internal class ReportBuilder
    {
        private const int BUILD_NAME = 0;
        private const int FAILED = 0;
        private const int SUCCESS = 1;
        private const int IGNORED = 2;

        public void Build()
        {
            List<List<string>> files = new List<List<string>>();
            var output = new List<string>();
            var testsCountByResults = new List<int> { 0, 0, 0 };
            var errorsToTests = new Dictionary<string, List<string>>();
            List<string>  passList= new List<string>();
            int[] sum = { 0, 0, 0 };

            addFilesToList(files);
            output.Add(string.Format("DateTime: {0}\n", DateTime.Now.ToString("dd/MM/yyy")));

            foreach (var file in files)
            {
                if (file.Count > 0)
                {
                    errorsToTests.Add(file[BUILD_NAME], new List<string>());
                    SumFirstLines(file[1], sum);
                    AddFailures(file, testsCountByResults, errorsToTests, passList);
                }
            }

            int all = testsCountByResults[FAILED] + testsCountByResults[SUCCESS] + testsCountByResults[IGNORED];
            int all2 = sum[FAILED] + sum[SUCCESS] + sum[IGNORED];
            int coverage = (all > 0) ? (testsCountByResults[FAILED] + testsCountByResults[SUCCESS])*100/all : 0;
            var sb = new StringBuilder();

            sb.AppendFormat("Tests: {0}, ", all);
            sb.AppendFormat("Failed: {0}, ", testsCountByResults[FAILED]);
            sb.AppendFormat("Success: {0}, ", testsCountByResults[SUCCESS]);
            sb.AppendFormat("Ignored: {0}, ", testsCountByResults[IGNORED]);
            sb.AppendFormat("Coverage: {0}% \n", coverage);
            output.Add(sb.ToString());

            output.Add("By first line: All: " + all2 + " Failed: " + sum[0] + " Success: " + sum[1] + " Ignored: " +
                       sum[2] + "\n");

            foreach (KeyValuePair<string, List<string>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var testNames = errorToTests.Value;

                output.Add(string.Format("{0}: ", errorName));
                foreach (string testName in testNames)
                {
                    output.Add(string.Format("         {0}. {1}", testsCounter++, testName));
                }
            }

            File.WriteAllLines("c:/DailyReport/output.txt", output);
        }

        private static void AddFailures(List<string> lines, List<int> testsCount, Dictionary<string, List<string>> errors, List<string> passList)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(" + Test result: Success"))
                {
                    string test = lines[i + 2];
                    passList.Add(test);
                    testsCount[SUCCESS]++;
                }
                else if (lines[i].Contains("Test ignored: "))
                {
                    testsCount[IGNORED]++;
                }
                else if (lines[i].Contains(" + Test result: Fail"))
                {
                    if (lines[i + 5].Contains("SkipException"))
                    {
                        testsCount[IGNORED]++;
                    }
                    else
                    {
                        string test = lines[i + 2].Replace("ERROR: Test failed: ", "");
                        test += " (" + lines[0] + ")";
                        i += 4;
                        
                        string error = lines[i].Replace(" + ","");
                        Regex rgx = new Regex("[a-zA-Z]+\\.[a-zA-Z]+\\.");
                        error = rgx.Replace(error, "");
                        if (!errors.ContainsKey(error))
                        {
                            errors.Add(error, new List<string>());
                        }

                        if (!errors[error].Contains(test))
                        {
                            testsCount[FAILED]++;
                            errors[error].Add(test);
                        }
                    }
                }
            }
        }


        private static void SumFirstLines(string firstLine, int[] sum)
        {
            string str = firstLine;
            var match = Regex.Match(str, @".*failed: (\d+).*passed: (\d+).*ignored: (\d+).*");
            int fa, su, ig;
            if (match.Groups.Count == 4)
            {
                int.TryParse(match.Groups[1].ToString(), out fa);
                int.TryParse(match.Groups[2].ToString(), out su);
                int.TryParse(match.Groups[3].ToString(), out ig);
                sum[FAILED] += fa;
                sum[SUCCESS] += su;
                sum[IGNORED] += ig;
            }
        }



        private static void addFilesToList(List<List<string>> files)
        {
            files.Add(
                new List<string>(
                    new List<string> {"TechnicianView"}.Concat(File.ReadAllLines("c:/DailyReport/TechnicianView.txt",
                        Encoding.UTF8))));
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
        }
    }
}
