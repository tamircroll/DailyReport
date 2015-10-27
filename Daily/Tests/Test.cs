using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Daily
{
    public class Test
    {
        private string linkToLogzIO;

        public Test(string name, string build, string buildNumber, string linkToLogzIo)
        {
            Name = name;
            Build = build;
            BuildNumber = buildNumber;
            LinkToLogzIO = linkToLogzIo;
        }

        public string Name { get; set; }
        public string Build { get; set; }
        public string BuildNumber { get; set; }

        public string LinkToLogzIO
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