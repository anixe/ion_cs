using BenchmarkDotNet.Attributes;
using System;

namespace Anixe.Ion.Benchmark
{
  public class IonReaderBenchmark
  {
    [Benchmark]
    public int Read()
    {
      using (var reader = IonReaderFactory.Create(FileLoader.GetExamplesIonPath()))
      {
        int total = 0;
        while (reader.Read())
        {
          if (reader.CurrentSection == "STATION")
          {
            if (reader.IsTableDataRow)
            {
              var row = reader.CurrentRawLine.AsSpan();
              total += row.Length;
            }
          }
        }
        return total;
      }
    }
  }
}