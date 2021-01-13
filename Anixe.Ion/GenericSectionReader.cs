using System;

namespace Anixe.Ion
{
  public class SectionReaderArgs
  {
    public string? SectionName { get; set; }
  }

  /// <summary>
  /// The class is a generic event handler which allows you to hook on ion file's sections
  /// </summary>
  public class GenericSectionReader
  {
    public event Action<object, SectionReaderArgs>? OnReadSection;
    private readonly IIonReader reader;
    public GenericSectionReader(IIonReader reader)
    {
      this.reader = reader;
    }

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