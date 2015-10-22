using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Daily.Exceptions;
using Daily.Tests;

namespace Daily
{
    public class MessageBuilder
    {
        public readonly string Message, SomeVersion;
        public TestsHandler TestsHandler;
        public readonly List<string> Builds;

        public MessageBuilder()
        {
            Builds = BuildHandler.getAllBuildsNumbers(new FilesHandler().getAllAndroidFiles());
            Message = build();
            SomeVersion = setSomeVersion();
            _replacePlaceHolders = new ReplacePlaceHolders(this);
        }

        private string setSomeVersion()
        {
            List<string> file = new FilesHandler().getAllAndroidFiles()[0];
            return VersionsHandler.getVersion(file);
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
            TestsHandler = new TestsHandler();
            var files = new FilesHandler().getAllAndroidFiles();
            List<string> suitesVersions = VersionsHandler.getsuitesVersions(files);
            _output.AddRange(suitesVersions);
            List<int> testsSummaryByTeamCity = getAllSuitesTestsSummaries(files);
            setTestsHandler(TestsHandler, files);

            setOutput(TestsHandler, testsSummaryByTeamCity);
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

                _output.Add(string.Format("{0}: {1}", errorName, ReplacePlaceHolders.LINE));
                foreach (Test test in tests)
                {
                    string failIndicator = test.isFirstTimeToGetError(errorName, FilesHandler.getNameByBuilds(Builds))? "*" : "";

                    _output.Add(string.Format("{0}{0}{0}{0}{1}{2}. {3}{4}{5}", ReplacePlaceHolders.SPACE,
                        ReplacePlaceHolders.SPAN_SMALL, testsCounter++, failIndicator, test, ReplacePlaceHolders.CLOSE_SPAN));
                }
                _output.Add(ReplacePlaceHolders.LINE);
            }
        }

        private void addSuiteToTestsHandler(List<string> fileLines, TestsHandler testsHandler, string suiteName)
        {
            string build = BuildHandler.getBuildNumber(fileLines);
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains("] Test ignored:") || fileLines[i].Contains("marked as Skipped"))
                {
                    testsHandler.addIgnored(new Test(fileLines[i] + i, suiteName, build));
                }

                if (!fileLines[i].StartsWith(" Test name: ")) continue;

                i += 2;
                if (fileLines[i].Contains("Success"))
                {
                    testsHandler.addPassed(new Test(fileLines[i] + i, suiteName, build));
                }

                else if (fileLines[i].Contains("Fail"))
                {
                    i += 4;
                    if (fileLines[i].Contains("SkipException"))
                    {
                        testsHandler.addIgnored(new Test(fileLines[i] + i, suiteName, build));
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
            string build = BuildHandler.getBuildNumber(fileLines);
            string testName = fileLines[i - 6].Replace(" Test name: ", "");
            testName = string.Format("{0}{1}{2}{3} ({4}){2}", Daily.ReplacePlaceHolders.SPAN_RED, testName,
                ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPAN_GREEN, fileLines[0]);

            string error = fileLines[i];
            testName += TestHandler.GetEndOfTestName(ref error, fileLines, ref i);
            ErrorHandler.setErrorName(ref error, fileLines, ref i);
            testsHandler.addFailure(error, new Test(testName, suiteName, build));
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
