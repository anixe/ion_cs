using NUnit.Framework;
using System.IO;

namespace Anixe.Ion.UnitTests
{
  public class SectionReaderTest
  {
    [Test]
    public void Should_Read_Multiple_Sections()
    {
      var counter = 0;
      using (var stream = File.OpenRead(FileLoader.GetInsIonPath()))
      using (var reader = IonReaderFactory.Create(stream))
      {
        var sectionReader = new GenericSectionReader(reader);
        sectionReader.OnReadSection += (sender, args) =>
        {
          Assert.That(args.SectionName, Is.AnyOf("META", "INSURANCE"));
          counter++;
        };
        sectionReader.Read();
      }
      Assert.AreEqual(2, counter);
    }
  }
}