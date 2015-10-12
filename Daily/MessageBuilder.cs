using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Daily.Tests;

namespace Daily
{
    public class MessageBuilder
    {
        public readonly string Message;

        public MessageBuilder()
        {
            Message = build();
            _replacePlaceHolders = new ReplacePlaceHolders(this);
        }

        public ReplacePlaceHolders ReplacePlaceHolders
        {
            get { return _replacePlaceHolders; }
        }

        private const int FAILED = 0, SUCCESS = 1, IGNORED = 2;
        private readonly List<string> _output = new List<string>();
        private readonly ReplacePlaceHolders _replacePlaceHolders;

        private string build()
        {
            TestsHandler testsHandler = new TestsHandler();
            var files = getAllAndroidFiles();
            List<int> testsSummaryByTeamCity = getAllSuitesTestsSummaries(files);
            setTestsHandler(testsHandler, files);

            setOutput(testsHandler, testsSummaryByTeamCity);
            return string.Concat(_output.ToArray());
        }

        private void setOutput(TestsHandler testsHandler, List<int> testsSummaryByTeamCity)
        {
            addTestsSummaryToOutput("Actual count", testsHandler.getTestsCount());
            addTestsSummaryToOutput("By Suite ", testsSummaryByTeamCity);

            _output.Add(String.Format("{2}{0}Issues with application:{1}", ReplacePlaceHolders.DIV_BOLD_UNDERLINE,
                ReplacePlaceHolders.CLOSE_DIV, ReplacePlaceHolders.LINE));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithApp());
            _output.Add(String.Format("{2}{0}Automation development failures:{1}",
                ReplacePlaceHolders.DIV_BOLD_UNDERLINE, ReplacePlaceHolders.CLOSE_DIV,
                ReplacePlaceHolders.LINE));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithAutomation());
            _output.Add(String.Format("{2}{0}UnKnown:{1}", Daily.ReplacePlaceHolders.DIV_BOLD_UNDERLINE,
                ReplacePlaceHolders.CLOSE_DIV, ReplacePlaceHolders.LINE));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithUnKnown());
        }

        private void setTestsHandler(TestsHandler testsHandler, List<List<string>> files)
        {
            foreach (var file in files)
            {
                string suiteName = file[0];
                addSuiteToTestsHandler(file, testsHandler, suiteName);
            }
        }

        private List<int> getAllSuitesTestsSummaries(List<List<string>> files)
        {
            var testSummaryBySuiteCount = new List<int> {0, 0, 0};
            foreach (var file in files)
            {
                if (file.Count > 3)
                {
                    addSuiteTestsSummary(file[3], testSummaryBySuiteCount);
                }
            }
            return testSummaryBySuiteCount;
        }

        private void addSuiteTestsSummary(string firstLine, List<int> testSummaryBySuiteCount)
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
                    testSummaryBySuiteCount[IGNORED] += ignore;
                }
                testSummaryBySuiteCount[FAILED] += fail;
                testSummaryBySuiteCount[SUCCESS] += success;
            }
        }

        private void addTestsSummaryToOutput(string title, List<int> testsCountByResults)
        {
            int all = testsCountByResults[FAILED] + testsCountByResults[SUCCESS] + testsCountByResults[IGNORED];
            int coverage = (all > 0) ? (testsCountByResults[FAILED] + testsCountByResults[SUCCESS])*100/all : 0;
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", title);
            sb.AppendFormat("Tests: {0}, ", all);
            sb.AppendFormat("Failed: {0}, ", testsCountByResults[FAILED]);
            sb.AppendFormat("Success: {0}, ", testsCountByResults[SUCCESS]);
            sb.AppendFormat("Ignored: {0}, ", testsCountByResults[IGNORED]);
            sb.AppendFormat("Coverage: {0}%", coverage);
            _output.Add(sb + ReplacePlaceHolders.LINE);
        }

        private void addErrorsDescriptionToOutput(SortedDictionary<string, List<Test>> errorsToTests)
        {
            _output.Add(ReplacePlaceHolders.LINE + ReplacePlaceHolders.LINE);
            foreach (KeyValuePair<string, List<Test>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var tests = errorToTests.Value;

                _output.Add(string.Format("{0}: {1}", errorName, Daily.ReplacePlaceHolders.LINE));
                foreach (Test testName in tests)
                {
                    _output.Add(string.Format("{4}{4}{4}{4}{0}{1}. {2}{3}", ReplacePlaceHolders.SPAN_SMALL,
                        testsCounter++, testName, ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPACE));
                }
                _output.Add(ReplacePlaceHolders.LINE);
            }
        }

        private void addSuiteToTestsHandler(List<string> fileLines, TestsHandler testsHandler, string suiteName)
        {
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains("] Test ignored:") || fileLines[i].Contains("marked as Skipped"))
                {
                    testsHandler.addIgnored(new Test(fileLines[i] + i, suiteName));
                }

                if (!fileLines[i].StartsWith(" Test name: ")) continue;

                i += 2;
                if (fileLines[i].Contains("Success"))
                {
                    testsHandler.addPassed(new Test(fileLines[i] + i, suiteName));
                }

                else if (fileLines[i].Contains("Fail"))
                {
                    i += 4;
                    if (fileLines[i].Contains("SkipException"))
                    {
                        testsHandler.addIgnored(new Test(fileLines[i] + i, suiteName));
                        i += 5;
                    }
                    else
                    {
                        doIfFail(fileLines, testsHandler, suiteName, ref i);
                    }
                }
            }
        }

        private void doIfFail(List<string> fileLines, TestsHandler testsHandler, string suiteName, ref int i)
        {
            string testName = fileLines[i - 6].Replace(" Test name: ", "");
            testName = string.Format("{0}{1}{2}{3} ({4}){2}", Daily.ReplacePlaceHolders.SPAN_RED, testName,
                ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPAN_GREEN, fileLines[0]);

            string error = fileLines[i];
            testName += GetEndOfTestName(ref error, fileLines, ref i);
            setErrorName(ref error, fileLines, ref i);
            testsHandler.addFailure(error, new Test(testName, suiteName));
        }

        private string GetEndOfTestName(ref string error, List<string> fileLines, ref int i)
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
            }

            return addToEndOfTestName + ReplacePlaceHolders.LINE;
        }

        private void setErrorName(ref string error, List<string> fileLines, ref int i)
        {
            if (
                error.Contains(
                    @"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for:"))
            {
                error = "TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition";
            }
            else if (
                error.Contains(
                    "concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: "))
            {
                error = "TimeoutException: NoSuchElementException: Couldn't find notification element by predicate";
            }
            else if (error.Contains("Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for:"))
            {
                error = "TimeoutException: Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for: Os report for device: [DeviceID]";
            }
            else if (
                error.Contains(
                    "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element for"))
            {
                error = "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element";
            }
            else if (error.Contains("Unable to provision, see the following errors"))
            {
                error = error.Replace(", see the following errors:", ". ");
                i += 4;
                error += fileLines[i]
                    .Replace(
                        "1) Error in custom provider, java.lang.Exception: Failed providing appium driver. Exception: org.openqa.selenium.WebDriverException: ",
                        "");
            }
            else
            {
                i++;
                int maxLinesToAdd = i + 3;
                while (!fileLines[i].Contains("Browser video download") &&
                       !fileLines[i].Contains("Test artifacts path:") && i < maxLinesToAdd)
                {
                    error += " " + fileLines[i++];
                }
            }
            while (error.EndsWith(".") || error.EndsWith(":") || error.EndsWith(" "))
            {
                error = error.Substring(0, error.Length - 1);
            }
        }

        private List<List<string>> getAllAndroidFiles()
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
//                new List<string>(
//                    new List<string> {"OngoingValue"}
//                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_Ongoing_Value.log", Encoding.UTF8))),
                new List<string>(
                    new List<string> {"TechExpertExperienceTests"}
                        .Concat(File.ReadAllLines("c:/DailyReport/E2E_Tests_-_Appium_Tech_Expert_Experience.log",
                            Encoding.UTF8)))
            };

            return files;
        }
    }

    public static class HtmlExtensions
    {
        public static string ToRawHtml(this string html)
        {
            return html
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}
