using System.Collections.Generic;
using System.Linq;

namespace Daily.Tests
{
    public class TestsHandler
    {
        private readonly SortedDictionary<string, List<Test>> _errorlsToFailedTests = new SortedDictionary<string, List<Test>>();
        private readonly List<Test> _passedTests = new List<Test>();
        private readonly List<Test> _ignoredTests = new List<Test>();
        
        public readonly List<string> Builds;
        public readonly string Versions;

        public TestsHandler(List<List<string>> files)
        {
            Builds = BuildHandler.getAllBuildsNumbers(files);
            Versions = VersionsHandler.getVersionsStr(files);
            foreach (var file in files)
            {
                string buildNumber = BuildHandler.getBuildNumber(file[1]);
                string suiteNameAndLink = file[0];
                List<List<string>> fileSplitedToTests = TestHelper.SplitFileToTests(file);
                addSuiteToTestsHandler(fileSplitedToTests, suiteNameAndLink, buildNumber);
            }
        }

        public override string ToString()
        {
            string toReturn = "";
            toReturn += addIssueTitle("Issues with application:");
            toReturn += getErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.Application));
            toReturn += addIssueTitle("Automation development failures:");
            toReturn += getErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.Automation));
            toReturn += addIssueTitle("UnKnown:");
            toReturn += getErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.UnKnown));
            return toReturn;
        }

        private void addSuiteToTestsHandler(List<List<string>> tests, string suiteName, string buildNumber)
        {
            foreach (List<string> test in tests)
            {
                Test tempTest = TestCreator.create(test, suiteName, buildNumber);
                if (tempTest != null) addTestToMap(tempTest);
            }
        }

        private void addTestToMap(Test test)
        {
            switch (test.Result)
            {
                case TestsResult.Failed:
                    addFailure(test.Error, test);
                    break;
                case TestsResult.Ignored:
                    _ignoredTests.Add(test);
                    break;
                case TestsResult.Success:
                    _passedTests.Add(test);
                    break;
            }
        }

        public SortedDictionary<string, List<Test>> FailedTests
        {
            get { return _errorlsToFailedTests; }
        }

        public List<int> getTestsCount()
        {
            return new List<int>
            {
                FailedTests.Keys.Sum(key => FailedTests[key].Count),
                _passedTests.Count,
                _ignoredTests.Count
            };
        }

        private void addFailure(string error, Test test)
        {
            if (!_errorlsToFailedTests.ContainsKey(error))
            {
                _errorlsToFailedTests.Add(error, new List<Test>());
            }

            if (!_errorlsToFailedTests[error].Contains(test))
            {
                _errorlsToFailedTests[error].Add(test);
            }
        }

        private SortedDictionary<string, List<Test>> getFailedTestsByIssue(IssueWith issueType)
        {
            SortedDictionary<string, List<Test>> toReturn = new SortedDictionary<string, List<Test>>();

            foreach (KeyValuePair<string, List<Test>> error in FailedTests)
            {
                if (IssuesFactory.Get(error.Key) == issueType)
                {
                    toReturn.Add(error.Key, error.Value);
                }
            }

            return toReturn;
        }

        private string addIssueTitle(string issuesWith)
        {
            return string.Format("{0}{1}" + issuesWith + "{2}", ReplacePlaceHolders.LINE, ReplacePlaceHolders.DIV_BOLD_UNDERLINE, ReplacePlaceHolders.CLOSE_DIV);
        }

        private string getErrorsDescriptionToOutput(SortedDictionary<string, List<Test>> errorsToTests)
        {
            string toRetrun = "";
            toRetrun += ReplacePlaceHolders.LINE;
            foreach (KeyValuePair<string, List<Test>> errorToTests in errorsToTests)
            {
                int testsCounter = 1;
                var errorName = errorToTests.Key;
                var tests = errorToTests.Value;

                toRetrun += string.Format("{0}: {1}", errorName, ReplacePlaceHolders.LINE);
                foreach (Test test in tests)
                {
                    string failIndicator = test.isFirstTimeToGetError(errorName, FilesHandler.getNameByBuilds(Builds))
                        ? "*"
                        : "";

                    toRetrun += string.Format(@"{0}{0}{0}{0}{1}{2}. {3}{4}. {5},   {6}{7}{8}", ReplacePlaceHolders.SPACE,
                        ReplacePlaceHolders.SPAN_SMALL, testsCounter++, failIndicator, test, test.Build, test.LinkToLogzIo,
                        ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.LINE);
                }
                toRetrun += ReplacePlaceHolders.LINE;
            }

            return toRetrun;
        }
    }
}
