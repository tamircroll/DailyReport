using System.Collections.Generic;
using System.Linq;

namespace Daily.Tests
{
    static class IssueWithFactory
    {
        public static IssueWith Get(string msg)
        {
            if (belongsToIssuePlatform(IssueWithAutomationPlatform, msg)) return IssueWith.Automation;
            if (belongsToIssuePlatform(IssueWithApp, msg)) return IssueWith.Application;
            return IssueWith.UnKnown;
        }

        static private bool belongsToIssuePlatform(List<string> issueWithApp, string msg)
        {
            return issueWithApp.Any(msg.Contains);
        }

        private static readonly List<string> IssueWithApp = new List<string>
        {
            "Timed out while waiting for: App ExDialer should be presented but it doesn't",
            "Couldn't find notification element by predicate:",
            "Timed out while waiting for: device brightness to be",
            "Timed out while waiting 40000ms for device status",
            "Page object of type ChatWaitWindow failed to load",
            "PageLoadException: Page object of type",
            "waiting for: Typing Gesture Shown On User View Wait Condition",
            "AssertionError",
            "ms for device status: Online",
            "BooleanWaitCondition Timed out while waiting for",
            "DeviceViewLoadedWaiter Timed out while",
            "TelemetryReceivedWaiter Timed out while waiting",
            "Brightness toggle wasn't visible after",
            "waiting for install addon button to become clickable",
            "Bluetooth toggle wasn't visible after ",
            "AnalyticsFetcherWaitCondition",
            "ConfigurationChangedOnDeviceWaiter Wait condition failed. Exception: java.lang.Exception:",
            "Timed out while waiting for: Auto Brightness Setting Waiter",
            "Timed out while waiting for: get notification if shown",
            "Page object of type EnterPhoneNumberPage failed to load",
            "Page object of type InCallPageObject failed to load",
            "The following notification was found, but was not supposed to",
            "Page object of type HomeViewPageObject failed to load. Missing elements",
        };

        static private readonly List<string> IssueWithAutomationPlatform = new List<string>
        {
            "Test initialization failed:",
            "Executed SSH Command failed on client",
            "Not logged in to playstore",
            "java.lang.NullPointerException",
            "WebDriverException",
            @"AppiumSeleniumAdb_Launcher\Resources\adb.exe -H",
            "Waiter Condition: Timed out while waiting for: Waiting for HomeTestsHelper service to be online",

        };
    }
}
