using System.Collections.Generic;

namespace Daily.Tests
{
    static class TestHelper
    {
        public static string GetEndOfTestName(string error)
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

            return addToEndOfTestName;
        }

        public static List<List<string>> SplitFileToTests(List<string> file)
        {
            List<List<string>> tests = new List<List<string>>();
            List<string> temp = new List<string>();
            foreach (string line in file)
            {
                if (line.StartsWith(" ++++++ Starting test:") || line.Contains("] Test ignored:"))
                {
                    tests.Add(temp);
                    temp = new List<string> { line };
                }
                else
                {
                    temp.Add(line);
                }
            }
            tests.Add(temp);
            tests.RemoveAt(0);
            return tests;
        }


    }
}
