using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    class MailReportBuilder
    {
        private const int FAILED = 0;
        private const int SUCCESS = 1;
        private const int IGNORED = 2;
        private const string BR = "</br>";

        List<string> output = new List<string>();

        public string Build()
        {
            List<List<string>> files = new List<List<string>>();
            
            var testsCountByResults = new List<int> { 0, 0, 0 };
            var errorsToTests = new Dictionary<string, List<string>>();
            List<string>  passList= new List<string>();
            int[] sumByFirstLine = { 0, 0, 0 };

            addFilesToList(files);
            output.Add(string.Format("</p>DateTime: {0}" + BR, DateTime.Now.ToString("dd/MM/yyy")));

            foreach (var file in files)
            {
                if (file.Count > 3)
                {
                    SumThirdLines(file[3], sumByFirstLine);
                    AddFailures(file, testsCountByResults, errorsToTests, passList);
                }
            }

            int all = testsCountByResults[FAILED] + testsCountByResults[SUCCESS] + testsCountByResults[IGNORED];
            int all2 = sumByFirstLine[FAILED] + sumByFirstLine[SUCCESS] + sumByFirstLine[IGNORED];
            int coverage = (all > 0) ? (testsCountByResults[FAILED] + testsCountByResults[SUCCESS])*100/all : 0;
            var sb = new StringBuilder();

            sb.AppendFormat("Tests: {0}, ", all);
            sb.AppendFormat("Failed: {0}, ", testsCountByResults[FAILED]);
            sb.AppendFormat("Success: {0}, ", testsCountByResults[SUCCESS]);
            sb.AppendFormat("Ignored: {0}, ", testsCountByResults[IGNORED]);
            sb.AppendFormat("Coverage: {0}% \n", coverage);
            output.Add(BR + sb.ToString() + BR + BR);
            
            foreach (KeyValuePair<string, List<string>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var testNames = errorToTests.Value;

                output.Add(string.Format("\n{0}: {1}", errorName, BR));
                foreach (string testName in testNames)
                {
                    output.Add(string.Format("<span style='font-size: 10pt'>&nbsp&nbsp&nbsp{0}. {1}</span>", testsCounter++, testName));
                }
                output.Add(BR);
            }
            output.Add("</p>");
            return string.Concat(output.ToArray());
        }

        private void AddFailures(List<string> lines, List<int> testsCount, Dictionary<string, List<string>> errors, List<string> passList)
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
                        test = "<span style = 'color:red'>" + test + "</span>";
                        test += "<span style = 'color:green'>" + " (" + lines[0] + ")" + "</span>";
                        i += linesToAddToGetError(lines, i);
                        
                        string error = lines[i].Replace(" + ","");
                        Regex rgx = new Regex("[a-zA-Z]+\\.[a-zA-Z]+\\.");
                        error = rgx.Replace(error, ""). Replace("Test exception: ", "");
                        test += setErrorName(ref error);
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

        private string setErrorName(ref string error)
        {
            string addToEndOfTestName = "";
            if (error.Contains(@"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for:"))
            {
                addToEndOfTestName = "[" + error.Replace(@"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for: ", "") + "]";
                error =  "TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition";
            }
            if (error.Contains("concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: "))
            {
                addToEndOfTestName =
                    error.Replace("concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: ", "")
                    .Replace(" Timed out while waiting for: get notification if shown.", "");
                error = "TimeoutException: NoSuchElementException: Couldn't find notification element by predicate";
            }
            if (error.Contains("selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element for"))
            {
                error = "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element";
            }

            return addToEndOfTestName + BR;
        }

        private int linesToAddToGetError(List<string> lines, int i)
        {
            if (lines[i + 4].Contains(
                "RuntimeException: Test initialization failed: Unable to provision, see the following errors"))
            {
                return 8;
            }

            return 4;
        }

        private void SumThirdLines(string firstLine, int[] sum)
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

        private void addFilesToList(List<List<string>> files)
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
                    new List<string> {"OngoingValue"}.Concat(File.ReadAllLines("c:/DailyReport/OngoingValue.txt",
                        Encoding.UTF8))));
            files.Add(
                new List<string>(
                    new List<string> {"TechExpertExperienceTests"}.Concat(
                        File.ReadAllLines("c:/DailyReport/techExpertExperienceTests.txt", Encoding.UTF8))));
        }
    }
}
