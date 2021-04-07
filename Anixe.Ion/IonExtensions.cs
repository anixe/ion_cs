using System;

namespace Anixe.Ion
{
  public static class IonExtensions
  {
    /// <summary>
    /// Creates ref-struct that represents key-value property
    /// </summary>
    /// <param name="reader">Instance of <see cref="IIonReader"/> with current state <see cref="IIonReader.IsProperty"/>=<see langword="true"/>.</param>
    /// <param name="propertySeparator">Character that separates key from value. By default it is used =.</param>
    /// <returns>An instance of <see cref="IonProperty"/> that has access to key and value spans in current line.</returns>
    /// <exception cref="InvalidOperationException">Reader is in not correct state. That means <see cref="IIonReader.IsProperty"/> is <see langword="false"/>.</exception>
    public static IonProperty ReadProperty(this IIonReader reader, char propertySeparator = '=')
    {
      return new IonProperty(reader, propertySeparator);
    }
  }
}
