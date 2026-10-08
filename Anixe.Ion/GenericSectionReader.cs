using System;

namespace Anixe.Ion
{
  /// <summary>
  /// The class represents the arguments for the section reader event.
  /// </summary>
  public class SectionReaderArgs
  {
        /// <summary>
        /// Gets or sets the name of the section that was read.
        /// This property is set when the event is triggered and contains
        /// the name of the section header that was read from the ion file.
        /// </summary>
        public string? SectionName { get; set; }
  }

  /// <summary>
  /// The class is a generic event handler which allows you to hook on ion file's sections
  /// </summary>
  public class GenericSectionReader
  {
    /// <summary>
    /// The event is triggered when a section header is read.
    /// The event handler receives the section name as an argument.
    /// </summary>
    public event Action<object, SectionReaderArgs>? OnReadSection;
    private readonly IIonReader reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericSectionReader"/> class.
    /// </summary>
    /// <param name="reader">The ion reader.</param>
    public GenericSectionReader(IIonReader reader)
    {
      this.reader = reader;
    }

    /// <summary>
    /// Reads the ion file and triggers the <see cref="OnReadSection"/> event for each section header encountered.
    /// </summary>
    public void Read()
    {
      while (reader.Read())
      {
        if (reader.IsSectionHeader)
        {
          var sectionName = reader.CurrentSection;
          OnReadSection?.Invoke(this, new SectionReaderArgs { SectionName = sectionName });
        }
      }
    }
  }
}