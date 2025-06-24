using System.IO;
using Xunit;

namespace Anixe.Ion.UnitTests
{
  public class SectionReaderTest
  {
    [Fact]
    public void Should_Read_Multiple_Sections()
    {
      var counter = 0;
      using (var stream = File.OpenRead(FileLoader.GetInsIonPath()))
      using (var reader = IonReaderFactory.Create(stream))
      {
        var sectionReader = new GenericSectionReader(reader);
        sectionReader.OnReadSection += (sender, args) =>
        {
          Assert.Contains(args.SectionName, new[] { "META", "INSURANCE" });
          counter++;
        };
        sectionReader.Read();
      }
      Assert.Equal(2, counter);
    }
  }
}