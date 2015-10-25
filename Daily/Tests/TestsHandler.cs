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

        public TestsHandler(List<List<string>> files)
        {
            foreach (var file in files)
            {
                string suiteName = file[0];
                addSuiteToTestsHandler(file, suiteName);
            }
        }

        public void addSuiteToTestsHandler(List<string> fileLines, string suiteName)
        {
            string build = BuildHandler.getBuildNumber(fileLines[1]);

            for (int i = 0; i < fileLines.Count; i++)
            {
                if (fileLines[i].Contains("marked as Skipped")) continue;
                if (fileLines[i].Contains("] Test ignored:"))
                {
                    addIgnored(new Test(fileLines[i] + i, suiteName, build, ""));
                }

                if (!fileLines[i].StartsWith(" Test name: ")) continue;

                i += 2;
                if (fileLines[i].Contains("Success"))
                {
                    addPassed(new Test(fileLines[i] + i, suiteName, build, ""));
                }

                else if (fileLines[i].Contains("Fail"))
                {
                    i += 4;
                    if (fileLines[i].Contains("SkipException"))
                    {
                        addIgnored(new Test(fileLines[i] + i, suiteName, build, ""));
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
            testName = string.Format("{0}{1}{2}{3} ({4}){2}", ReplacePlaceHolders.SPAN_RED, testName,
                ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPAN_GREEN, fileLines[0]);

            string error = fileLines[i];
            testName += TestHelper.GetEndOfTestName(ref error, fileLines, ref i);
            ErrorHandler.setErrorName(ref error, fileLines, ref i);
            addFailure(error, new Test(testName, suiteName, build, logzIO));
        }

        private string getLogsIo(List<string> fileLines, int i)
        {
            int minLineToSearch = i - 50;
            while (i > minLineToSearch)
            {
                if (fileLines[i].Contains(" ++++++ Link to Logz.io (contains all logs) for test name: "))
                {
                    return fileLines[i + 1];
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
                getFailedCount(),
                PassedTests.Count,
                IgnoredTests.Count
            };
        }

        public int getFailedCount()
        {
            return FailedTests.Keys.Sum(key => FailedTests[key].Count);
        }

        public SortedDictionary<string, List<Test>> getIssuesWithApp()
        {
            return getFailedTestsByIssue(IssueWith.Application);
        }

        public SortedDictionary<string, List<Test>> getIssuesWithAutomation()
        {
            return getFailedTestsByIssue(IssueWith.Automation);
        }

        public SortedDictionary<string, List<Test>> getIssuesWithUnKnown()
        {
            return getFailedTestsByIssue(IssueWith.UnKnown);
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
    }
}
