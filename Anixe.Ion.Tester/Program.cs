using System;
using System.Diagnostics;
using System.IO;

namespace Anixe.Ion.Tester
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            int linesCount = 0;
            double fileSizeInMb = GetFileSizeInMB("example.ion");

            using(IIonReader reader = IonReaderFactory.Create("example.ion"))
            {
                stopwatch.Start();
                linesCount = ReadIonLines(reader);
                stopwatch.Stop();
            }

            Console.WriteLine("Performance analysis:");
            Console.WriteLine(string.Format("IonReader read {0}MB file containing {1} lines in {2}ms", fileSizeInMb, linesCount, stopwatch.ElapsedMilliseconds));
        }

        private static double GetFileSizeInMB(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            return fileInfo.Length / 1024.0d / 1024.0d;
        }

        private static int ReadIonLines(IIonReader reader)
        {
            int result = 0;

            while(reader.Read())
            {
                result++;
            }

            return result;
        }
    }
}
