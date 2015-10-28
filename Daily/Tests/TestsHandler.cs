using System.Collections.Generic;
using System.Linq;
using Daily.Exceptions;

namespace Daily.Tests
{
    public class TestsHandler
    {
        private readonly SortedDictionary<string, List<Test>> errorlsToFailedTests = new SortedDictionary<string, List<Test>>();
        private readonly List<Test> passedTests = new List<Test>();
        private readonly List<Test> ignoredTests = new List<Test>();
        private List<string> Builds;

        public TestsHandler(List<List<string>> files)
        {
            Builds = BuildHandler.getAllBuildsNumbers(files);
            foreach (var file in files)
            {
                string suiteNameAndLink = file[0];
                addSuiteToTestsHandler(file, suiteNameAndLink);
            }
        }

        public override string ToString()
        {
            string toReturn = "";
            toReturn += addIssueTitle("Issues with application:");
            toReturn += addErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.Application));
            toReturn += addIssueTitle("Automation development failures:");
            toReturn += addErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.Automation));
            toReturn += addIssueTitle("UnKnown:");
            toReturn += addErrorsDescriptionToOutput(getFailedTestsByIssue(IssueWith.UnKnown));
            return toReturn;
        }

        public void addSuiteToTestsHandler(List<string> fileLines, string suiteName)
        {
            string buildNumber = BuildHandler.getBuildNumber(fileLines[1]);

            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains("] Test ignored:"))
                {
                    addIgnored(new Test(fileLines[i] + i, suiteName, buildNumber, ""));
                }

                if (!fileLines[i].StartsWith(" Test name: ")) continue;

                i += 2;
                if (fileLines[i].Contains("Success"))
                {
                    addPassed(new Test(fileLines[i] + i, suiteName, buildNumber, ""));
                }

                else if (fileLines[i].Contains("Fail") && !fileLines[i].Contains("marked as Skipped"))
                {
                    i += 4;
                    if (fileLines[i].Contains("SkipException"))
                    {
                        addIgnored(new Test(fileLines[i] + i, suiteName, buildNumber, ""));
                        i += 5;
                    }
                    else
                    {
                        doIfFail(fileLines, suiteName, ref i);
                    }
                }
            }
        }

        private void doIfFail(List<string> fileLines, string suiteName, ref int i)
        {
            string logzIO = getLogsIo(fileLines, i);
            string build = BuildHandler.getBuildNumber(fileLines[1]);
            string testName = fileLines[i - 6].Replace(" Test name: ", "");
            testName = string.Format("{0}{1}{2}{3} {2}", ReplacePlaceHolders.SPAN_GREEN, testName,
                ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPAN_RED);

            string error = fileLines[i];
            testName += TestHelper.GetEndOfTestName(ref error);
            ErrorHandler.setErrorName(ref error, fileLines, ref i);
            addFailure(error, new Test(testName, suiteName, build, logzIO));
        }

        private string getLogsIo(List<string> fileLines, int i)
        {
            while (!fileLines[i].StartsWith(" ++++++ Starting test: "))
            {
                if (fileLines[i].StartsWith(@"https://goo.gl/"))
                {
                    return fileLines[i];
                }

                i--;
            }
            return "Could not find logzLio link";
        }

        public SortedDictionary<string, List<Test>> FailedTests
        {
            get { return errorlsToFailedTests; }
        }

        public List<Test> PassedTests
        {
            get { return passedTests; } 
        }

        public List<Test> IgnoredTests
        {
            get { return ignoredTests; } 
        }

        public void addFailure(string error, Test test)
        {
            if (!errorlsToFailedTests.ContainsKey(error))
            {
                errorlsToFailedTests.Add(error, new List<Test>());
            }

            if (!errorlsToFailedTests[error].Contains(test))
            {
                errorlsToFailedTests[error].Add(test);
            }
        }

        public void addPassed(Test test)
        {
            passedTests.Add(test);
        }

        public void addIgnored(Test test)
        {
            ignoredTests.Add(test);
        }

        public List<int> getTestsCount()
        {
            return new List<int>
            {
                FailedTests.Keys.Sum(key => FailedTests[key].Count),
                PassedTests.Count,
                IgnoredTests.Count
            };
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

        private string addErrorsDescriptionToOutput(SortedDictionary<string, List<Test>> errorsToTests)
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
                        ReplacePlaceHolders.SPAN_SMALL, testsCounter++, failIndicator, test, test.Build, test.LinkToLogzIO,
                        ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.LINE);
                }
                toRetrun += ReplacePlaceHolders.LINE;
            }

            return toRetrun;
        }
    }
}
