using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Daily.Exceptions;

namespace Daily.Tests
{
    static class TestCreator
    {
        public static Test create(List<string> testLines, string suiteName, string buildNumber)
        {
            if (retriedTest(testLines)) return null;

            TestsResult result = getTestResult(testLines);

            if (result == TestsResult.Ignored) return new Test(testLines[0], result, suiteName, buildNumber, "", "");
            
            string exception = result == TestsResult.Failed ? getException(testLines) : "";
            string testName = getTestName(testLines, result, exception);
            string linkToLogzIO = result == TestsResult.Failed ? getLLinkToLogzIO(testLines) : "";

            return new Test(testName, result, suiteName, buildNumber, linkToLogzIO, exception);
        }

        private static string getException(List<string> testLines)
        {
            string exception = getLineThatStartWith(testLines, " Test exception: ");
            ErrorHandler.setErrorName(ref exception, testLines);

            return exception;
        }

        private static bool retriedTest(List<string> testLines)
        {
            return testLines.Any(line => line.Contains("Fail") && line.Contains("marked as Skipped"));
        }

        private static string getLLinkToLogzIO(List<string> testLines)
        {
            string link = getLineThatStartWith(testLines, @"https://goo.gl/");
            return new LinkCreator().makeLink(@"logz.Io Link", link);
        }

        private static string getTestName(List<string> testLines, TestsResult result, string exception)
        {
            const string startWith = " Test name: ";
            string line = getLineThatStartWith(testLines, startWith);
            string testName = line.Replace(startWith, "");
            testName = string.Format("{0}{1}{2}{3} {2}", ReplacePlaceHolders.SPAN_GREEN, testName,
                ReplacePlaceHolders.CLOSE_SPAN, ReplacePlaceHolders.SPAN_RED);
            testName += result == TestsResult.Failed ? TestHelper.GetEndOfTestName(exception) : "";

            return testName;
        }
        
        private static TestsResult getTestResult(List<string> testLines)
        {
            if (testLines.Any(testLine => testLine.Contains("] Test ignored:"))) return TestsResult.Ignored;

            string line = getLineThatStartWith(testLines, " Test result: ");

            if (line.Contains("Success")) return TestsResult.Success;
            if (line.Contains("Fail")) return TestsResult.Failed;

            throw new Exception("couldn't find test result");
        }
        
        private static string getLineThatStartWith(List<string> testLines, string startWith)
        {
            foreach (string line in testLines)
            {
                if (line.StartsWith(startWith)) return line;
            }

            return "";
        }
    }
}
