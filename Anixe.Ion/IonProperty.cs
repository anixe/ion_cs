using System;

namespace Anixe.Ion
{
  /// <summary>
  /// Represents a ref-struct that accesses key and value of property of <see cref="IIonReader"/>.
  /// </summary>
  public ref struct IonProperty
  {
    public readonly ReadOnlySpan<char> Key;
    public readonly ReadOnlySpan<char> Value;

    /// <summary>
    /// Creates a new instance of <see cref="IonProperty"/>.
    /// </summary>
    /// <param name="reader">Instance of <see cref="IIonReader"/> with current state <see cref="IIonReader.IsProperty"/>=<see langword="true"/>.</param>
    /// <param name="propertySeparator">Character that separates key from value. By default it is used =.</param>
    /// <exception cref="InvalidOperationException">Reader is in not correct state. That means <see cref="IIonReader.IsProperty"/> is <see langword="false"/>.</exception>
    public IonProperty(IIonReader reader, char propertySeparator = '=')
    {
      if (!reader.IsProperty)
      {
        throw new InvalidOperationException("Cannot read property when reader in not on property line.");
      }

      var span = (ReadOnlySpan<char>)reader.CurrentRawLine.AsSpan();
      var sepIdx = span.IndexOf(propertySeparator);
      if (sepIdx == -1)
      {
        this.Key = ReadOnlySpan<char>.Empty;
        this.Value = span;
        return;
      }

      this.Key = span.Slice(0, sepIdx).Trim().Trim('"');
      this.Value = span.Slice(sepIdx + 1, span.Length - sepIdx - 1).Trim().Trim('"');
    }
  }
}
