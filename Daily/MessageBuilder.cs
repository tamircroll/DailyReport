using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daily
{
    public class MessageBuilder
    {
        private const string
            LINE = "{0}",
            SPACE = "{1}",
            SPAN_SMALL = "{2}",
            SPAN_RED = "{3}",
            SPAN_GREEN = "{4}",
            CLOSE_SPAN = "{5}",
            DIV_BOLD_UNDERLINE = "{6}",
            CLOSE_DIV = "{7}"; 
                                     

        private List<int> actualTestsSummary = new List<int> {0, 0, 0};
        private string message;

        public MessageBuilder()
        {
            message = build();
        }

        public string GetTextMessage()
        {
            return message
                .Replace(SPAN_SMALL, "")
                .Replace(SPAN_RED, "")
                .Replace(SPAN_GREEN, "")
                .Replace(CLOSE_SPAN, "")
                .Replace(LINE, "\n")
                .Replace(DIV_BOLD_UNDERLINE, "")
                .Replace(CLOSE_DIV, "")
                .Replace(SPACE, " ");
        }

        public string GetHtmlMessage()
        {
            return message.ToRawHtml()
                .Replace(SPAN_SMALL, "<span style='font-size: 10pt'>")
                .Replace(SPAN_RED, "<span style = 'color:red'>")
                .Replace(SPAN_GREEN, "<span style = 'color:green'>")
                .Replace(CLOSE_SPAN, "</span>")
                .Replace(LINE, "<br>")
                .Replace(DIV_BOLD_UNDERLINE, "<div style='text-decoration: underline; font-weight: bold;'>")
                .Replace(CLOSE_DIV, "</div>")
                .Replace(SPACE, "&nbsp");
        }

        private const int FAILED = 0, SUCCESS = 1, IGNORED = 2;
        private readonly List<string> _output = new List<string>();

        public string build()
        {
            var files = getAllFiles();
            var testsSummaryBySuite = getAllSuitesTestsSummaries(files);
            var errorsToTests = getErrorsToTestsMap(files);

            addTestsSummaryToOutput("Actual count", actualTestsSummary);
            addTestsSummaryToOutput("By Suite ", testsSummaryBySuite);

            _output.Add(String.Format("{2}{0}Issues with application:{1}", DIV_BOLD_UNDERLINE, CLOSE_DIV, LINE));
            addErrorsDescriptionToOutput(errorsToTests);
            _output.Add(String.Format("{2}{0}Automation development failures:{1}", DIV_BOLD_UNDERLINE, CLOSE_DIV, LINE));
            return string.Concat(_output.ToArray());
        }

        private SortedDictionary<string, List<string>> getErrorsToTestsMap(List<List<string>> files)
        {
            var errorsToTests = new SortedDictionary<string, List<string>>();
            foreach (var file in files)
            {
                addFailures(file, errorsToTests, actualTestsSummary);
            }

            return errorsToTests;
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
            _output.Add(sb + LINE);
        }

        private void addErrorsDescriptionToOutput(SortedDictionary<string, List<string>> errorsToTests)
        {
            _output.Add(LINE + LINE);
            foreach (KeyValuePair<string, List<string>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var testNames = errorToTests.Value;

                _output.Add(string.Format("{0}: {1}", errorName, LINE));
                foreach (string testName in testNames)
                {
                    _output.Add(string.Format("{4}{4}{4}{4}{0}{1}. {2}{3}", SPAN_SMALL, testsCounter++, testName,
                        CLOSE_SPAN, SPACE));
                }
                _output.Add(LINE);
            }
        }

        private void addFailures(List<string> fileLines, SortedDictionary<string, List<string>> errors,
            List<int> testsCount)
        {
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains("marked as Skipped"))
                {
                    testsCount[IGNORED]--;
                    continue;
                }

                if (fileLines[i].Contains(" + Test result: Success"))
                {
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
                        i += 5;
                    }
                    else
                    {
                        doIfFail(fileLines, errors, testsCount, ref i);
                    }
                }
            }
        }

        private void doIfFail(List<string> fileLines, SortedDictionary<string, List<string>> errors,
            List<int> testsCount, ref int i)
        {
            string test = fileLines[i - 1].Replace("+ Test name: ", "");
            test = string.Format("{0}{1}{2}{3} ({4}){2}", SPAN_RED, test, CLOSE_SPAN, SPAN_GREEN, fileLines[0]);
            i += BuildOutputHelper.linesToAddToGetError(fileLines, i);

            string error = fileLines[i].Replace(" + ", "");
            var rgx = new Regex("[a-zA-Z]+\\.[a-zA-Z]+\\.");
            error = rgx.Replace(error, "").Replace("Test exception: ", "");
            test += setErrorName(ref error, fileLines, ref i);
            addTestToMap(errors, testsCount, error, test);
        }

        private static void addTestToMap(SortedDictionary<string, List<string>> errors, List<int> testsCount,
            string error, string test)
        {
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

        private string setErrorName(ref string error, List<string> fileLines, ref int i)
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
            else if (error.Contains("Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for:"))
            {
                error =
                    "TimeoutException: Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for: Os report for device: [DeviceID]";
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
            if (error.EndsWith(".") || error.EndsWith(":"))
            {
                error = error.Substring(0, error.Length - 1);
            }

            return addToEndOfTestName + LINE;
        }

        private List<List<string>> getAllFiles()
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
