using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace Anixe.Ion.Benchmark
{
    public class Config : ManualConfig
  {
    public Config()
    {
      Add(DefaultConfig.Instance);
      AddDiagnoser(MemoryDiagnoser.Default);
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      var summary = BenchmarkRunner.Run<IonReaderBenchmark>(new Config{});
    }
  }
}
