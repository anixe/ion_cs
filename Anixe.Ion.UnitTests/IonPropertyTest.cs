using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Anixe.Ion.UnitTests
{
  public class IonPropertyTest
  {
    [TestCase("some_key = some_value", "some_key", "some_value")]
    [TestCase("\"some_key\" = \"some_value\"", "some_key", "some_value")]
    [TestCase("\"some_key\" = 1", "some_key", "1")]
    [TestCase(" \"some_key\" = \"some_value\" ", "some_key", "some_value")]
    [TestCase("\"some_key\": \"some_value\"", "", "\"some_key\": \"some_value\"")]
    [TestCase("\"\" = \"\"", "", "")]
    [TestCase(" = ", "", "")]
    [TestCase("some key = some value", "some key", "some value")]
    [TestCase("key=some=value", "key", "some=value")]
    [TestCase("some_setting = \"true\"", "some_setting", "true")]
    [TestCase("some_setting =", "some_setting", "")]
    public void IonProperty_Test(string input, string key, string value)
    {
      using var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      Assert.True(reader.IsProperty);

      var property = new IonProperty(reader);
      Assert.AreEqual(key, property.Key.ToString());
      Assert.AreEqual(value, property.Value.ToString());
    }

    [TestCase("")]
    [TestCase("[AAA]")]
    [TestCase("|--|--|")]
    public void IonProperty_Throws_InvalidOperationException(string input)
    {
      using var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      reader.Read();
      Assert.False(reader.IsProperty);
      Assert.Throws<InvalidOperationException>(() => new IonProperty(reader));
    }
  }
}
