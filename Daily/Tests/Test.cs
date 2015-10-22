using System.Collections.Generic;
using System.Linq;

namespace Daily
{
    public class Test
    {
        public Test(string name, string suite, string buildNumber)
        {
            Name = name;
            Suite = suite;
            BuildNumber = buildNumber;
        }

        public string Name { get; set; }
        public string Suite { get; set; }
        public string BuildNumber { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool isFirstTimeToGetError(string error, string currFileName)
        {
            List<List<string>> files = FilesHandler.getAllFilesFromDirectory(@"C:\DailyReport\OldReports", currFileName);
            foreach (var file in files)
            {
                if (file.Any(x => x.Equals(FilesHandler.setErrorAndTestName(error, Name))))
                    return true;
            }
            return false;
        }
    }
}
