# IonReader

## Introduction
Anixe.Ion library is a netstandard2.0 library that provides reader with fast, non-cached, forward only access to *.ion files and writer to build ion file content. 

## Features
### IonReaderFactory
Anixe.Ion library provides static factory which can create an instance for **IonReader** interface. Factory enables two ways of creation the **IonReader** instance:

1. With path to the file. File is opened with: **FileMode.Open**, **FileAccess.Read**, **FileShare.ReadWrite**. Possible argument exceptions:
   * **ArgumentException** with message "File path must be defined!" when given file path is null, empty or contains only white spaces.
   * **ArgumentException** with message "File 'XXX' does not exist!" when given XXX path does not exist on disk.
2. With Stream.

#### IonReader interface
##### Properties
* **CurrentLine** gets current line value. It causes new string allocation from CurrentRawLine 
* **CurrentRawLine** gets current line value as ArraySegment<char>. It is allocation free. Data is from rented buffer.
* **CurrentLineNumber** gets current line number
* **CurrentSection** gets information about current section name. Its value will change only when **CurrentLine** is on line which begins with *'['*
* **IsSectionHeader** gets boolean value indicating whether first character of **CurrentLine** is *'['*
* **IsComment** gets boolean value indicating whether first character of **CurrentLine** is *'#'*
* **IsTableRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'*
* **IsTableHeaderRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'* and current table header was not already passed
* **IsTableHeaderSeparatorRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'* and second character is *'-'*
* **IsTableDataRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'* and current table header was already passed
* **IsEmptyLine** returns boolean value indicating whether first character of **CurrentLine** is empty string or line is filled with empty spaces
* **IsProperty** returns boolean value indicating whether other properties are false

##### Methods
* **Read** reads each *.ion file line and returns boolean value indicating whether reading was successful or not
* **Dispose** calls **Dispose** method of the underlying stream

#### GenericSectionReader class

##### Events
* **OnReadSection** the event handler fired for each section with section name in args
##### Methods
* **Read** the main method of the class, it reads the stream provided by IonReader and fires an event for each section

## Example use
### With file path
```c#
class MainClass
{
    public static void Main(string[] args)
    {
        using(IIonReader reader = IonReaderFactory.Create("/path/to/example.ion"))
        {
            ReadIon(reader);
        }
    }

    private static void ReadIon(IIonReader reader)
    {
        while(reader.Read())
        {
            Console.WriteLine(reader.CurrentLine);
        }
    }
}
```
### With stream
```c#
class MainClass
{
    public static void Main(string[] args)
    {
        using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using(IIonReader reader = IonReaderFactory.Create(fileStream))
            {
                ReadIon(reader);
            }
        }
    }

    private static void ReadIon(IIonReader reader)
    {
        while(reader.Read())
        {
            Console.WriteLine(reader.CurrentLine);
        }
    }
}
```

### With SectionReader
```c#
class MainClass
{
    public static void Main(string[] args)
    {
        using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using(IIonReader reader = IonReaderFactory.Create(fileStream))
            {
                ReadIon(reader);
            }
        }
    }

    private static void ReadIon(IIonReader reader)
    {
        var sectionReader = new GenericSectionReader(reader);
        sectionReader.OnReadSection += (sender, args) =>
        {
          //TODO: check args.SectionName to determine current section
        };
        sectionReader.Read();
    }
}
```
