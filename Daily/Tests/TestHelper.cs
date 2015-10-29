namespace Daily.Tests
{
    class TestHelper
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
    }
}
