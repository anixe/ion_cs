using System;

namespace Anixe.Ion
{
  /// <summary>
  /// Represents a ref-struct that accesses key and value of property of <see cref="IIonReader"/>.
  /// </summary>
  public readonly ref struct IonProperty
  {
    /// <summary>
    /// Gets the key of the property. It is a read-only span of characters
    /// that represents the key part of the property line in the ion file.
    /// </summary>
    public readonly ReadOnlySpan<char> Key;

    /// <summary>
    /// Gets the value of the property. It is a read-only span of characters
    /// that represents the value part of the property line in the ion file.
    /// </summary>
    public readonly ReadOnlySpan<char> Value;

    /// <summary>
    /// Deconstructs the <see cref="IonProperty"/> into its key and value components.
    /// </summary>
    /// <param name="Key">The key component.</param>
    /// <param name="Value">The value component.</param>
    public void Deconstruct(out ReadOnlySpan<char> Key, out ReadOnlySpan<char> Value)
    {
        Key = this.Key;
        Value = this.Value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="IonProperty"/>.
    /// </summary>
    /// <param name="reader">Instance of <see cref="IIonReader"/> with current state <see cref="IIonReader.IsProperty"/>=<see langword="true"/>.</param>
    /// <param name="propertySeparator">Character that separates key from value. By default it is used = character.</param>
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
      this.Value = span.Slice(sepIdx + 1).Trim().Trim('"');
    }
  }
}
