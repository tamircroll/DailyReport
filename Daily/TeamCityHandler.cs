using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Daily
{
    public class TeamCityHandler
    {
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
                    testSummaryBySuiteCount[MessageBuilder.IGNORED] += ignore;
                }
                testSummaryBySuiteCount[MessageBuilder.FAILED] += fail;
                testSummaryBySuiteCount[MessageBuilder.SUCCESS] += success;
            }
        }
    }
}