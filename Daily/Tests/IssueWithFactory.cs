using System.Collections.Generic;

namespace Daily.Tests
{
    static class IssueWithFactory
    {
        public static IssueWith Get(string msg)
        {
            if (belongsToIssuePlatform(issueWithAutomationPlatform, msg)) return IssueWith.Automation;
            if (belongsToIssuePlatform(issueWithApp, msg)) return IssueWith.Application;
            return IssueWith.UnKnown;
        }

        static private bool belongsToIssuePlatform(List<string> issueWithApp, string msg)
        {
            foreach (string error in issueWithApp)
            {
                if (msg.Contains(error)) return true;
            }

            return false;
        }

        static List<string> issueWithApp = new List<string>
        {
            "Timed out while waiting for: App ExDialer should be presented but it doesn't",
            "Couldn't find notification element by predicate:",
            "Timed out while waiting for: device brightness to be",
            "Timed out while waiting 40000ms for device status",
            "Page object of type ChatWaitWindow failed to load",
            "PageLoadException: Page object of type",
            "waiting for: Typing Gesture Shown On User View Wait Condition",
            "AssertionError",
        };

        static List<string> issueWithAutomationPlatform = new List<string>
        {
            "Test initialization failed:",
            "Executed SSH Command failed on client",
            "Not logged in to playstore",
            "java.lang.NullPointerException",
        };

    }
}
