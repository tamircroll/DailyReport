using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    internal class MessageBuilder
    {
        public const string
            START_PARAGRAPH = "{0}",
            CLOSE_PARAGRAPH = "{1}",
            SPAN_SMALL = "{2}",
            SPAN_RED = "{3}",
            SPAN_GREEN = "{4}",
            CLOSE_SPAN = "{5}",
            LINE = "{6}";
        private const int FAILED = 0, SUCCESS = 1, IGNORED = 2;
        private List<string> output = new List<string>();

        public string Build()
        {
            var successTests = new List<string>();
            List<int> actualTestsSummary = new List<int> { 0, 0, 0 };
            var files = getAllFiles();
            var testSummaryBySuiteCount = getAllThirdLinesSummaries(files);

            output.Add(START_PARAGRAPH);

            var errorsToTests = getAllFailures(files, actualTestsSummary, successTests);

            addTestsSummaryToOutput("Actual count" , actualTestsSummary);
            addTestsSummaryToOutput("By Suite", testSummaryBySuiteCount);
            addErrorsDescriptionToOutput(errorsToTests);

            output.Add(CLOSE_PARAGRAPH);
            return string.Concat(output.ToArray());
        }


        private Dictionary<string, List<string>> getAllFailures(List<List<string>> files, List<int> actualTestsSummary, List<string> successTests)
        {
            var errorsToTests = new Dictionary<string, List<string>>();
            foreach (var file in files)
            {
                AddFailures(file, errorsToTests, successTests, actualTestsSummary);
            }

            return errorsToTests;
        }

        private List<int> getAllThirdLinesSummaries(List<List<string>> files)
        {
            var testSummaryBySuiteCount = new List<int>(){0,0,0};
            foreach (var file in files)
            {
                if (file.Count > 3)
                {
                    addThirdLineSummary(file[3], testSummaryBySuiteCount);
                }
            }
            return testSummaryBySuiteCount;
        }

        private void addTestsSummaryToOutput(string title, List<int> testsCountByResults)
        {
            int all = testsCountByResults[FAILED] + testsCountByResults[SUCCESS] + testsCountByResults[IGNORED];
            int coverage = (all > 0) ? (testsCountByResults[FAILED] + testsCountByResults[SUCCESS])*100/all : 0;
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", title);
            sb.AppendFormat("All Tests: {0}, ", all);
            sb.AppendFormat("Failed: {0}, ", testsCountByResults[FAILED]);
            sb.AppendFormat("Success: {0}, ", testsCountByResults[SUCCESS]);
            sb.AppendFormat("Ignored: {0}, ", testsCountByResults[IGNORED]);
            sb.AppendFormat("Coverage: {0}%", coverage);
            output.Add(sb + LINE);


        }

        private void addErrorsDescriptionToOutput(Dictionary<string, List<string>> errorsToTests)
        {
            output.Add(LINE + LINE);
            foreach (KeyValuePair<string, List<string>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var testNames = errorToTests.Value;

                output.Add(string.Format("{0}: {1}", errorName, LINE));
                foreach (string testName in testNames)
                {
                    output.Add(string.Format("{0}{1}. {2}{3}", SPAN_SMALL, testsCounter++, testName, CLOSE_SPAN));
                }
                output.Add(LINE);
            }
        }

        private void AddFailures(List<string> fileLines, Dictionary<string, List<string>> errors, List<string> passList, List<int> testsCount)
        {
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains(" + Test result: Success"))
                {
                    string test = fileLines[i + 2];
                    passList.Add(test);
                    testsCount[SUCCESS]++;
                }
                else if (fileLines[i].Contains("Test ignored: "))
                {
                    testsCount[IGNORED]++;
                }
                else if (fileLines[i].Contains(" + Test result: Fail"))
                {
                    if (fileLines[i + 5].Contains("SkipException"))
                    {
                        testsCount[IGNORED]++;
                    }
                    else
                    {
                        doIfFail(fileLines, errors, testsCount, ref i);
                    }
                }
            }
        }

        private void doIfFail(List<string> fileLines, Dictionary<string, List<string>> errors, List<int> testsCount, ref int i)
        {
            string test = fileLines[i + 2].Replace("ERROR: Test failed: ", "");
            test = SPAN_RED + test + CLOSE_SPAN;
            test += SPAN_GREEN + " (" + fileLines[0] + ")" + CLOSE_SPAN;
            i += BuildOutputHelper.linesToAddToGetError(fileLines, i);

            string error = fileLines[i].Replace(" + ", "");
            Regex rgx = new Regex("[a-zA-Z]+\\.[a-zA-Z]+\\.");
            error = rgx.Replace(error, "").Replace("Test exception: ", "");
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

        private string setErrorName(ref string error)
        {
            string addToEndOfTestName = "";
            if (
                error.Contains(
                    @"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for:"))
            {
                addToEndOfTestName = "[" +
                                     error.Replace(
                                         @"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for: ",
                                         "") + "]";
                error = "TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition";
            }
            else if (
                error.Contains(
                    "concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: "))
            {
                addToEndOfTestName =
                    error.Replace(
                        "concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: ",
                        "")
                        .Replace(" Timed out while waiting for: get notification if shown.", "");
                error = "TimeoutException: NoSuchElementException: Couldn't find notification element by predicate";
            }
            else if (
                error.Contains(
                    "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element for"))
            {
                error =
                    "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element";
            }
            else if (error.EndsWith(".") || error.EndsWith(":"))
                error = error.Substring(0, error.Length - 1);

            return addToEndOfTestName + LINE;
        }

        private void addThirdLineSummary(string firstLine, List<int> sum)
        {
            string str = firstLine;
            var match = Regex.Match(str, @".*failed: (\d+).*passed: (\d+).*ignored: (\d+).*");
            if (match.Groups.Count < 4) match = Regex.Match(str, @".*failed: (\d+).*passed: (\d+).*");
            if (match.Groups.Count >= 3)
            {
                int fail, success, ignore;
                int.TryParse(match.Groups[1].ToString(), out fail);
                int.TryParse(match.Groups[2].ToString(), out success);
                if (match.Groups.Count >= 4)
                {
                    int.TryParse(match.Groups[3].ToString(), out ignore);
                    sum[IGNORED] += ignore;
                }
                sum[FAILED] += fail;
                sum[SUCCESS] += success;
            }
        }

        private List<List<string>> getAllFiles()
        {
            List<List<string>> files = new List<List<string>>();

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

            return files;
        }
    }
}
