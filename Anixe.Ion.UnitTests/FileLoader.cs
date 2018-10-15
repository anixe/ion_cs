using System;
using System.IO;
using System.Linq;

namespace Anixe.Ion.UnitTests
{
  public static class FileLoader
  {
        public readonly static string ExamplesFileFolderPath;
        public readonly static string ProjectRootDir;

        static FileLoader()
        {
            ProjectRootDir = LookupForProjectRoot();
            ExamplesFileFolderPath = Path.Combine(ProjectRootDir, "Examples");
        }

        private static string LookupForProjectRoot()
        {
            var slnDir = GetSlnDir();
            if (!string.IsNullOrEmpty(slnDir))
            {
                return Path.Combine(slnDir, "Anixe.Ion.UnitTests");
            }
            throw new Exception("Cannot find sln dir.");
        }

        private static string GetSlnDir()
        {
            var currDir = Environment.CurrentDirectory;
            if (Directory.EnumerateFiles(currDir, "*.sln").Any())
            {
                return currDir;
            }
            while (Directory.GetParent(currDir) != null && !Directory.EnumerateFiles(currDir, "*.sln").Any())
            {
                currDir = Directory.GetParent(currDir).FullName;
            }
            return currDir;
        }

        public static string GetInsGzIonPath()
        {
            return Path.Combine(ExamplesFileFolderPath, "ins.ion.gz");
        }

        public static string GetExamplesIonPath()
        {
            return Path.Combine(ExamplesFileFolderPath, "example.ion");
        }

        public static string GetInsIonPath()
        {
            return Path.Combine(ExamplesFileFolderPath, "ins.ion");
        }

        public static string GetIonWithWindowsLineEndings()
        {
            return "[DEFAULTS]\r\ncurrency = \"EUR\"\r\nlanguage = \"de\"\r\nsales_market = \"DE\"\r\ntimeout = 5000\r\nsipp = \"ECAR\"\r\nprice_margin = %5\r\ndisabled_log_urls = [ \"/G\", \"/cars\", \"/cars_group_by\" ]\r\nenabled_log_urls = [ ]";
        }
    }
}