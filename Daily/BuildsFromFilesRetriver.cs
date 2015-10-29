using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daily.Build;
using Daily.Io;

namespace Daily
{
    public class BuildsFromFilesRetriver : IBuildRetriver
    {
        private LatestLogPathFiner mLatestLogPathFiner;
        

        public BuildsFromFilesRetriver()
        {
            mLatestLogPathFiner = new LatestLogPathFiner();
        }

        public List<TcBuild> Get()
        {
            var list = new List<TcBuild>
            {
                GetTestsList("TechnicianView"),
                GetTestsList("FirstExperience"),
                GetTestsList("OngoingValue"),
                GetTestsList("TechExpertExperience"),
                GetTestsList("MultiDevicesTeam"),
                GetTestsList("EnableMorePartners")
            };

            list.RemoveAll(item => item == null);
            getAndUpdateArtifactsLink(list);
            return list;
        }

        private void getAndUpdateArtifactsLink(List<TcBuild> builds)
        {
            foreach (var build in builds)
            {
                build.Link = BuildHandler.getBuildAsLink(build.SuiteName, build.Log);
            }
        }

        private TcBuild GetTestsList(string suite)
        {
            var builder = new StringBuilder();
            foreach (var c in suite)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            var suiteSplittedByCapitel = builder.ToString();

            try
            {
                return new TcBuild
                {
                    SuiteName = suite,
                    Log = File.ReadAllLines(mLatestLogPathFiner.Find(suiteSplittedByCapitel), Encoding.UTF8).ToList()
                };

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<List<string>> getAllFilesFromDirectory(string folderPath, string desclude)
        {
            var toReturn = new List<List<string>>();
            var filesPaths = Directory.GetFiles(folderPath).ToList();

            foreach (var path in filesPaths)
            {
                if (!path.Contains(desclude))
                {
                    toReturn.Add(new List<string>(File.ReadAllLines(path, Encoding.UTF8)));
                }
            }

            return toReturn;
        }

        public static string setErrorAndTestName(string error, string test)
        {
            return error + " " + test;
        }

        public static string getNameByBuilds(List<string> builds)
        {
            string name = "";
            foreach (string build in builds)
            {
                name += build + "_";
            }

            return name.Remove(name.Length - 1) + ".txt";
        }
    }
}