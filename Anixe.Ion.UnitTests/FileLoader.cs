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
            if (Directory.EnumerateFiles(currDir, "*.slnx").Any())
            {
                return currDir;
            }
            while (Directory.GetParent(currDir) != null && !Directory.EnumerateFiles(currDir, "*.slnx").Any())
            {
                currDir = Directory.GetParent(currDir)!.FullName;
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
            return "[DEFAULTS]\r\n" +
                "currency = \"EUR\"\r\n" +
                "language = \"de\"\r\n" +
                "sales_market = \"DE\"\r\n" +
                "timeout = 5000\r\n" +
                "sipp = \"ECAR\"\r\n" +
                "price_margin = %5\r\n" +
                "disabled_log_urls = [ \"/G\", \"/cars\", \"/cars_group_by\" ]\r\n" +
                "enabled_log_urls = [ ]";
        }
    }
}