# IonReader

## Introduction
Anixe.Ion library is a .NET Standard 1.6 library that provides reader with fast, non-cached, forward only access to *.ion files. 

## How to build

Just use VisualStudio or dotnet core command line interface:

```
cd <project_dir>
dotnet restore
dotnet build Anixe.Ion
```

## How to test

Just use VisualStudio test view or dotnet core command line interface:

```
cd <project_dir>
dotnet restore
dotnet test Anixe.Ion.UnitTests
```

## Features
#### IonReaderFactory
Anixe.Ion library provides static factory which can create an instance for **IonReader** interface. Factory enables two ways of creation the **IonReader** instance:

1. With path to the file. File is opened with: **FileMode.Open**, **FileAccess.Read**, **FileShare.ReadWrite**. Possible argument exceptions:
   * **ArgumentException** with message "File path must be defined!" when given file path is null, empty or contains only white spaces.
   * **ArgumentException** with message "File 'XXX' does not exist!" when given XXX path does not exist on disk.
2. With Stream.

#### IonReader interface
##### Properties
* **CurrentLine** gets current line value
* **CurrentLineNumber** gets current line number
* **CurrentSection** gets information about current section name. Its value will change only when **CurrentLine** is on line which begins with *'['*
* **IsSectionHeader** gets boolean value indicating whether first character of **CurrentLine** is *'['*
* **IsComment** gets boolean value indicating whether first character of **CurrentLine** is *'#'*
* **IsTableRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'*
* **IsTableHeaderSeparatorRow** gets boolean value indicating whether first character of **CurrentLine** is *'|'* and second character is *'-'*
* **IsEmptyLine** returns boolean value indicating whether first character of **CurrentLine** is empty string or line is filled with empty spaces
* **IsProperty** returns boolean value indicating whether other properties are false

##### Methods
* **Read** reads each *.ion file line and returns boolean value indicating whether reading was successful or not
* **Dispose** calls **Dispose** method of the underlying stream
 
## Example use
#### With file path
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
#### With stream
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

## More examples

Please check tests:
 * {project_dir}/Anixe.Ion.UnitTests/SectionReaderTest.cs
 * {project_dir}/Anixe.Ion.UnitTests/TableReaderTest.cs