using System;
using System.Collections.Generic;
using System.Text;
using Daily.Build;
using Daily.Tests;

namespace Daily
{
    public class MessageBuilder
    {
        public readonly string Message;
        public readonly TestsHandler TestsHandler;
        public readonly ReplacePlaceHolders ReplacePlaceHolders;
        public const int FAILED = 0;
        public const int SUCCESS = 1;
        public const int IGNORED = 2;

        private readonly List<string> _output = new List<string>();
        private readonly List<TcBuild> _builds;

        public MessageBuilder(List<TcBuild> builds)
        {
            _builds = builds;
            TestsHandler = new TestsHandler(_builds);
            Message = buildMessage();
            ReplacePlaceHolders = new ReplacePlaceHolders(this);
        }

        private string buildMessage()
        {
            addSuitesVersionsToOutput();
            addSummeariesToOutput();
            _output.Add(TestsHandler.ToString());

            return string.Concat(_output.ToArray());
        }

        private void addSuitesVersionsToOutput()
        {
            List<string> suitesVersions = VersionsHandler.getsuitesVersions(_builds);
            _output.AddRange(suitesVersions);
        }

        private void addSummeariesToOutput()
        {
            addTestsSummaryToOutput(string.Format("By Build{0}{0}{0}{0}{0}{0}", ReplacePlaceHolders.SPACE),
                new TeamCityHandler().getAllSuitesTestsSummaries(_builds));
            addTestsSummaryToOutput("Actual count", TestsHandler.getTestsCount());
        }

        private void addTestsSummaryToOutput(string title, List<int> testsCountByResults)
        {
            int all = testsCountByResults[FAILED] + testsCountByResults[SUCCESS] + testsCountByResults[IGNORED];
            int coverage = (all > 0) ? (testsCountByResults[FAILED] + testsCountByResults[SUCCESS])*100/all : 0;
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", title);
            sb.AppendFormat("Tests: {0}, ", all);
            sb.AppendFormat("Failed: {0}, ", testsCountByResults[FAILED]);
            sb.AppendFormat("Success: {0}, ", testsCountByResults[SUCCESS]);
            sb.AppendFormat("Ignored: {0}, ", testsCountByResults[IGNORED]);
            sb.AppendFormat("Coverage: {0}%", coverage);
            _output.Add(sb + ReplacePlaceHolders.LINE);
        }
    }

    public static class HtmlExtensions
    {
        public static string ToRawHtml(this string html)
        {
            return html
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}
