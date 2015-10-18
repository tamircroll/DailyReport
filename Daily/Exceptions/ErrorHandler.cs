using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.Exceptions
{
    static class ErrorHandler
    {
        public static void setErrorName(ref string error, List<string> fileLines, ref int i)
        {
            if (
                error.Contains(
                    @"concurrent.TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition Timed out while waiting for:"))
            {
                error = "TimeoutException: Waiter Condition: AnalyticsFetcherWaitCondition";
            }
            else if (
                error.Contains(
                    "concurrent.TimeoutException: Waiter Condition:  Wait condition failed. Exception: NoSuchElementException: Couldn't find notification element by predicate: "))
            {
                error = "TimeoutException: NoSuchElementException: Couldn't find notification element by predicate";
            }
            else if (error.Contains("Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for:"))
            {
                error =
                    "TimeoutException: Waiter Condition: TelemetryReceivedWaiter Timed out while waiting for: Os report for device: [DeviceID]";
            }
            else if (
                error.Contains(
                    "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element for"))
            {
                error = "selenium.TimeoutException: Timed out after 120 seconds waiting for visibility of Proxy element";
            }
            else if (error.Contains("Unable to provision, see the following errors"))
            {
                error = error.Replace(", see the following errors:", ". ");
                i += 4;
                error += fileLines[i]
                    .Replace(
                        "1) Error in custom provider, java.lang.Exception: Failed providing appium driver. Exception: org.openqa.selenium.WebDriverException: ",
                        "");
            }
            else if (
                error.Contains(
                    "A new session could not be created. (Original error: UiAutomator quit before it successfully launched)"))
            {
                error =
                    "Test exception: java.lang.RuntimeException: Test initialization failed: java.util.concurrent.ExecutionException: java.lang.RuntimeException: org.openqa.selenium.SessionNotCreatedException: A new session could not be created. (Original error: UiAutomator quit before it successfully launched) ";
            }
            else if (error.Contains("WebDriverException: Error forwarding the new session Error forwarding the request Connection reset Command duration or timeout"))
            {
                error = "Test exception: java.lang.RuntimeException: Test initialization failed: java.util.concurrent.ExecutionException: java.lang.RuntimeException: org.openqa.selenium.WebDriverException: Error forwarding the new session Error forwarding the request Connection reset Command duration or timeout:";
            }
            else if (error.Contains("Test exception: java.lang.RuntimeException: Test initialization failed: java.util.concurrent.ExecutionException: java.lang.RuntimeException: org.openqa.selenium.WebDriverException: Error forwarding the new session Error forwarding the request"))
            {
                error = "Test exception: java.lang.RuntimeException: Test initialization failed: java.util.concurrent.ExecutionException: java.lang.RuntimeException: org.openqa.selenium.WebDriverException: Error forwarding the new session Error forwarding the request Connection reset Command duration or timeout";
            }
            else if (error.Contains("WebDriverException: The path to the driver executable must be set by the webdriver.chrome.driver system property; for more information, see https://github.com/SeleniumHQ/selenium/wiki/ChromeDriver. "))
            {
                error = "WebDriverException: The path to the driver executable must be set by the webdriver.chrome.driver system property; for more information, see https://github.com/SeleniumHQ/selenium/wiki/ChromeDriver. ";
            }
            else
            {
                i++;
                int maxLinesToAdd = i + 3;
                while (!fileLines[i].Contains("Browser video download") &&
                       !fileLines[i].Contains("Test artifacts path:") && i < maxLinesToAdd)
                {
                    error += " " + fileLines[i++];
                }
            }
            while (error.EndsWith(".") || error.EndsWith(":") || error.EndsWith(" "))
            {
                error = error.Substring(0, error.Length - 1);
            }
        }
    }
}
