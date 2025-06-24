using System;
using System.IO;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests
{
  public class IonPropertyTest
  {
    [Theory]
    [InlineData("some_key = some_value", "some_key", "some_value")]
    [InlineData("\"some_key\" = \"some_value\"", "some_key", "some_value")]
    [InlineData("\"some_key\" = 1", "some_key", "1")]
    [InlineData(" \"some_key\" = \"some_value\" ", "some_key", "some_value")]
    [InlineData("\"some_key\": \"some_value\"", "", "\"some_key\": \"some_value\"")]
    [InlineData("\"\" = \"\"", "", "")]
    [InlineData(" = ", "", "")]
    [InlineData("some key = some value", "some key", "some value")]
    [InlineData("key=some=value", "key", "some=value")]
    [InlineData("some_setting = \"true\"", "some_setting", "true")]
    [InlineData("some_setting =", "some_setting", "")]
    public void IonProperty_Test(string input, string key, string value)
    {
      using var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      Assert.True(reader.IsProperty);

      var property = new IonProperty(reader);
      Assert.Equal(key, property.Key.ToString());
      Assert.Equal(value, property.Value.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("[AAA]")]
    [InlineData("|--|--|")]
    public void IonProperty_Throws_InvalidOperationException(string input)
    {
      using var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      Assert.False(reader.IsProperty);
      Assert.Throws<InvalidOperationException>(() => new IonProperty(reader));
    }
  }
}
