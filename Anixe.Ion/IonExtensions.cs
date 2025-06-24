using System;

namespace Anixe.Ion
{
  public static class IonExtensions
  {
    /// <summary>
    /// Creates ref-struct that represents key-value property.
    /// </summary>
    /// <param name="reader">Instance of <see cref="IIonReader"/> with current state <see cref="IIonReader.IsProperty"/>=<see langword="true"/>.</param>
    /// <param name="propertySeparator">Character that separates key from value. By default it is used = character.</param>
    /// <returns>An instance of <see cref="IonProperty"/> that has access to key and value spans in current line.</returns>
    /// <exception cref="InvalidOperationException">Reader is in not correct state. That means <see cref="IIonReader.IsProperty"/> is <see langword="false"/>.</exception>
    public static IonProperty ReadProperty(this IIonReader reader, char propertySeparator = '=')
    {
      return new IonProperty(reader, propertySeparator);
    }

    /// <summary>
    /// Creates ref-struct that allows reading table row cells in an efficient way.
    /// It does not advance the reader to the next line.
    /// </summary>
    /// <param name="reader">Instance of <see cref="IIonReader"/>.</param>
    /// <returns>A new instance of <see cref="TableRowReader"/>.</returns>
    /// <exception cref="InvalidOperationException">Current line is not a table row.</exception>
    public static TableRowReader ReadTableRow(this IIonReader reader)
    {
      if (!reader.IsTableRow)
      {
        throw new InvalidOperationException("Cannot read table row when reader is not on table row line.");
      }

      return new TableRowReader(reader.CurrentRawLine);
    }
  }
}
