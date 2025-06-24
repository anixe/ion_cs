using System;
using System.IO;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests
{
  public class IonExtensionsTest
  {
    [Fact]
    public void ReadProperty_Test()
    {
      var reader = CreateReaderForInput("\"some_key\" = \"some_value\"");
      var ionProperty = reader.ReadProperty();

      ReadOnlySpan<char> key = ionProperty.Key;
      ReadOnlySpan<char> value = ionProperty.Value;

      Assert.Equal("some_key", key.ToString());
      Assert.Equal("some_value", value.ToString());

      // getting more than one time
      Assert.Equal("some_key", ionProperty.Key.ToString());

      // reading property one more time
      var newIonProperty = reader.ReadProperty();
      Assert.Equal("some_key", newIonProperty.Key.ToString());
    }

    [Theory]
    [InlineData(" = ")]
    [InlineData("\"\" = \"\"")]
    public void ReadProperty_Empty_Property_Test(string input)
    {
      var reader = CreateReaderForInput(input);
      var ionProperty = reader.ReadProperty();

      Assert.Equal(string.Empty, ionProperty.Key.ToString());
      Assert.Equal(string.Empty, ionProperty.Value.ToString());
    }

    private static IIonReader CreateReaderForInput(string input)
    {
      var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      return reader;
    }
  }
}
