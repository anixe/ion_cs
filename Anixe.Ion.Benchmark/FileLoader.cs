using System;
using System.IO;
using System.Linq;

namespace Anixe.Ion.Benchmark
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
        return Path.Combine(slnDir, "Anixe.Ion.Benchmark");
      }
      throw new Exception("Cannot find sln dir.");
    }

    private static string GetSlnDir()
    {
        var dir = new DirectoryInfo(Environment.CurrentDirectory ?? ".");
        while (dir != null)
        {
            try
            {
                if (dir.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
                    return dir.FullName;
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we can't access and continue searching upward.
            }
            catch (IOException)
            {
                // Ignore IO errors and continue searching upward.
            }
            dir = dir.Parent;
        }

        // If no solution file was found, fall back to the original current directory.
        return Environment.CurrentDirectory ?? Directory.GetCurrentDirectory();
    }

    public static string GetExamplesIonPath()
    {
      return Path.Combine(ExamplesFileFolderPath, "stations.ion");
    }
  }
}