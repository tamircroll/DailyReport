using System.Collections.Generic;

namespace Daily.Tests
{
    class TestsHandler
    {
        private readonly SortedDictionary<string, List<Test>> errorlsToFailedTests = new SortedDictionary<string, List<Test>>();
        private readonly List<Test> passedTests = new List<Test>();
        private readonly List<Test> ignoredTests = new List<Test>();

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
            int count = 0;
            foreach (string key in FailedTests.Keys)
            {
                count += FailedTests[key].Count;
            }
            
            return count;
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
                if (IssueWithFactory.Get(error.Key) == issueType)
                {
                    toReturn.Add(error.Key, error.Value);
                }
            }

            return toReturn;
        }
    }
}
