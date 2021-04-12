using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Anixe.Ion.UnitTests
{
  public class IonExtensionsTest
  {
    [Test]
    public void ReadProperty_Test()
    {
      var reader = CreateReaderForInput("\"some_key\" = \"some_value\"");
      var ionProperty = reader.ReadProperty();

      ReadOnlySpan<char> key = ionProperty.Key;
      ReadOnlySpan<char> value = ionProperty.Value;

      Assert.AreEqual("some_key", key.ToString());
      Assert.AreEqual("some_value", value.ToString());

      // getting more than one time
      Assert.AreEqual("some_key", ionProperty.Key.ToString());

      // reading property one more time
      var newIonProperty = reader.ReadProperty();
      Assert.AreEqual("some_key", newIonProperty.Key.ToString());
    }

    [Test]
    public void ReadProperty_Empty_Property_Test()
    {
      var reader = CreateReaderForInput("\"\" = \"\"");
      var ionProperty = reader.ReadProperty();
      Assert.AreEqual("", ionProperty.Key.ToString());
      Assert.AreEqual("", ionProperty.Value.ToString());

      var secondReader = CreateReaderForInput(" = ");
      Assert.AreEqual("", ionProperty.Key.ToString());
      Assert.AreEqual("", ionProperty.Value.ToString());
    }

    private static IIonReader CreateReaderForInput(string input)
    {
      var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      return reader;
    }
  }
}
