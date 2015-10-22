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
        public readonly TestsHandler TestsHandler;
        public readonly List<string> Builds;
        public readonly ReplacePlaceHolders ReplacePlaceHolders;

        private const int FAILED = 0, SUCCESS = 1, IGNORED = 2;
        private readonly List<string> _output = new List<string>();
        private readonly List<List<string>> _files;

        public MessageBuilder(List<List<string>> files)
        {
            _files = files;
            TestsHandler = new TestsHandler(_files);
            Builds = BuildHandler.getAllBuildsNumbers(_files);
            Message = build();
            SomeVersion = getFirstFileVersion();
            ReplacePlaceHolders = new ReplacePlaceHolders(this);
        }

        private string getFirstFileVersion()
        {
            List<string> file = _files[0];
            return VersionsHandler.getVersion(file);
        }

        private string build()
        {
            List<string> suitesVersions = VersionsHandler.getsuitesVersions(_files);
            _output.AddRange(suitesVersions);
            List<int> testsSummaryByTeamCity = getAllSuitesTestsSummaries(_files);

            setOutput(TestsHandler, testsSummaryByTeamCity);
            return string.Concat(_output.ToArray());
        }

        private void setOutput(TestsHandler testsHandler, List<int> testsSummaryByTeamCity)
        {
            addTestsSummaryToOutput("Actual count", testsHandler.getTestsCount());
            addTestsSummaryToOutput("By Suite ", testsSummaryByTeamCity);

            _output.Add(String.Format("{0}{1}Issues with application:{2}", ReplacePlaceHolders.LINE, ReplacePlaceHolders.DIV_BOLD_UNDERLINE, ReplacePlaceHolders.CLOSE_DIV));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithApp());
            _output.Add(String.Format("{0}{1}Automation development failures:{2}", ReplacePlaceHolders.LINE, ReplacePlaceHolders.DIV_BOLD_UNDERLINE,
                ReplacePlaceHolders.CLOSE_DIV));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithAutomation());
            _output.Add(String.Format("{0}{1}UnKnown:{2}", ReplacePlaceHolders.LINE,
                ReplacePlaceHolders.DIV_BOLD_UNDERLINE, ReplacePlaceHolders.CLOSE_DIV));
            addErrorsDescriptionToOutput(testsHandler.getIssuesWithUnKnown());
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

        public List<int> getAllSuitesTestsSummaries(List<List<string>> files)
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
                    string failIndicator = test.isFirstTimeToGetError(errorName, FilesHandler.getNameByBuilds(Builds))
                        ? "*"
                        : "";

                    _output.Add(string.Format(@"{0}{0}{0}{0}{1}{2}. {3}{4}. Link: {5}{6}{7}", ReplacePlaceHolders.SPACE,
                        ReplacePlaceHolders.SPAN_SMALL, testsCounter++, failIndicator, test, test.LinkToLogzIO,
                        ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.LINE));
                }
                _output.Add(ReplacePlaceHolders.LINE);
            }
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
