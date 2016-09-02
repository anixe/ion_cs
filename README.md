# IonReader

## Introduction
Anixe.Ion library is a .NET 4.5 library that provides reader with fast, non-cached, forward only access to *.ion files. 

## Features
#### IonReaderFactory
Anixe.Ion library provides static factory which can create an instance for **IonReader** interface. Factory enables two ways of creating the **IonReader** instance:

1. With path to the file. File is opened with: *FileMode.Open*, *FileAccess.Read*, *FileShare.ReadWrite*. Possible argument exceptions in this case:
   * *ArgumentException* with message "File path must be defined!" when given file path is null, empty or contains only white spaces.
   * *ArgumentException* with message "File 'XXX' does not exist!" when given XXX path does not exist on disk.
2. With Stream.

#### IonReader interface
Anixe.Ion library defines following properties/methods:
* **Read** method which reads each *.ion file line and return boolean value indicating whether reading was successful or not
* **CurrentLine** string property which gets current line value
* **CurrentLineNumber** integer property which gets current line number
* **CurrentSection** string property which gets information about current section name. Its value will change only when **CurrentLine** is on line which begins with *'['*
* **IsSectionHeader** property which gets boolean value indicating whether first character of **CurrentLine** is *'['*
* **IsComment** property which gets boolean value indicating whether first character of **CurrentLine** is *'#'*
* **IsTableRow** property which gets boolean value indicating whether first character of **CurrentLine** is *'|'*
* **IsTableHeaderSeparatorRow** property which gets boolean value indicating whether first character of **CurrentLine** is *'|'* and second character is *'-'*
* **IsEmptyLine** property which returns boolean value indicating whether first character of **CurrentLine** is empty string or line is filled with empty spaces
* **IsProperty** property which returns boolean value indicating whether other properties are false
* **Dispose** which means that you can wrap it with *using* clausule.
 
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

