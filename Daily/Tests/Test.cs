using System.Collections.Generic;
using System.Linq;

namespace Daily
{
    public class Test
    {
        private string linkToLogzIO;

        public Test(string name, string suite, string buildNumber, string linkToLogzIo)
        {
            Name = name;
            Suite = suite;
            BuildNumber = buildNumber;
            LinkToLogzIO = linkToLogzIo;
        }

        public string Name { get; set; }
        public string Suite { get; set; }
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
            foreach (var file in files)
            {
                if (file.Any(x => x.Contains(FilesHandler.setErrorAndTestName(error, Name))))
                    return true;
            }
            return false;
        }
    }
}
