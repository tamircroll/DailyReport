using System.Collections.Generic;
using System.Linq;
using Daily.Tests;

namespace Daily
{
    public class Test
    {
        private string linkToLogzIO;

        public Test(string name, TestsResult result, string build, string buildNumber, string linkToLogzIo, string exception)
        {
            Name = name;
            Build = build;
            BuildNumber = buildNumber;
            LinkToLogzIo = linkToLogzIo;
            Result = result;
            Error = exception;
        }

        public string Name { get; set; }
        public string Build { get; private set; }
        public string BuildNumber { get; set; }
        public TestsResult Result { get; private set; }
        public string Error { get; private set; }

        public string LinkToLogzIo
        {
            get { return linkToLogzIO; }
            private set { linkToLogzIO = new LinkCreator().makeLink("logz.Io Link", value); }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool isFirstTimeToGetError(string error, string currFileName)
        {
            List<List<string>> files = FilesHandler.getAllFilesFromDirectory(@"C:\DailyReport\OldReports", currFileName);
            return files.Any(file => file.Any(line => line.Contains(FilesHandler.setErrorAndTestName(error, Name))));
        }
    }
}