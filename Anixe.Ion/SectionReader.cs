using System;
using System.Collections.Generic;

namespace Anixe.Ion
{
  public class SectionReader
  {
    private readonly IIonReader reader;
    private readonly Dictionary<string, Action<string>> sectionHandlers;
    public event Action<object, string> OnReadSection;

    public SectionReader(IIonReader reader, Dictionary<string, Action<string>> sectionHandlers = null)
    {
      this.reader = reader;
      this.sectionHandlers = sectionHandlers ?? new Dictionary<string, Action<string>> { };
    }

    public void RegisterHandler(string sectionName, Action<string> handler)
    {
      if (string.IsNullOrEmpty(sectionName))
      {
        throw new ArgumentNullException(nameof(sectionName));
      }
      if (handler == null)
      {
        throw new ArgumentNullException(nameof(handler));
      }
      if (this.sectionHandlers.ContainsKey(sectionName))
      {
        this.sectionHandlers[sectionName] += handler;
      }
      else
      {
        this.sectionHandlers[sectionName] = handler;
      }
    }

    public void Read()
    {
      while (reader.Read())
      {
        if (reader.IsSectionHeader)
        {
          var sectionName = this.reader.CurrentSection;
          this.OnReadSection?.Invoke(this, sectionName);
          FireHandler(sectionName);
        }
      }
    }

    private void FireHandler(string sectionName)
    {
      Action<string> handler = null;
      if (this.sectionHandlers != null && this.sectionHandlers.TryGetValue(sectionName, out handler))
      {
        handler(sectionName);
      }
    }

  }
}