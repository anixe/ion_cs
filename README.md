# IonReader

## Introduction
Anixe.Ion library is a .NET library that provides reader with fast, non-cached, forward only access to *.ion files and writer to build ion file content. 

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

## ION format specification
This section provides a formal specification of the textual ION format as implemented by the Anixe.Ion library. The specification defines lexical tokens, line classification, structural constructs, escaping rules and writer semantics.

1. Scope and overview

   ION is a line-oriented, human-readable representation comprised of zero or more sections. Each section may contain:
   - properties (key/value pairs),
   - comments,
   - optional tabular data expressed as pipe-delimited rows,
   - empty or whitespace-only lines.

2. Lexical tokens

   - '[' and ']' : section header delimiters.
   - '=' : property assignment operator.
   - '#' : comment prefix (only at the beginning of a line is recognized as a comment).
   - '|' : table column delimiter.
   - '-' : character used in header-separator rows.
   - '\\' : escape character for table cell content.
   - '"' : double-quote character for property string delimitation.
   - Newline representation: writer exposes NewLineEscaped ("\\n" or "\\r\\n" depending on environment) when configured to escape newlines.

3. Line classification (reader behavior)

   For each input line, the reader applies classification in this order:
   a. If the line begins with '[' then the line is a SectionHeader (IsSectionHeader=true).
   b. Else if the line begins with '#' then the line is a Comment (IsComment=true).
   c. Else if the line begins with '|' then the line is a TableRow (IsTableRow=true). Further distinctions:
      - If the second character is '-' then IsTableHeaderSeparatorRow=true.
      - If the current table header has not yet been encountered, the first matching '|' row is treated as IsTableHeaderRow=true.
      - Otherwise the line is a TableDataRow (IsTableDataRow=true).
   d. Else if the line is empty or contains only whitespace then IsEmptyLine=true.
   e. Otherwise the line is treated as a Property (IsProperty=true).

4. Sections

   - Syntax: A section header consists of a single line: '[' SECTION_NAME ']' where SECTION_NAME is the substring between the first '[' and the subsequent ']'.
   - Semantics: CurrentSection is updated only when the reader is positioned on a section header line.

5. Properties

   - Syntax: KEY '=' VALUE
     - KEY: characters from start of line up to the first '='; leading and trailing whitespace is removed by the reader.
     - VALUE: characters after the first '=' to the end of the line; leading and trailing whitespace is removed. If VALUE is enclosed in double quotes, the outer quotes are removed.
   - Quoting: Double quotes denote string values. Numeric and boolean literals may be written without quotes; the writer formats numeric values using invariant culture.
   - Reader API: IonProperty presents Key and Value as ReadOnlySpan<char> that are trimmed; surrounding double quotes are stripped from Value.

6. Comments and empty lines

   - Lines beginning with '#' are comments and ignored by higher-level consumers.
   - Lines consisting solely of whitespace are empty lines and may be used for separation of logical groups.

7. Tables

   7.1 Header row
   - A header row must start and end with '|' and contain one or more column names separated by '|' characters. Example:
     | col1 | col2 | col3 |

   7.2 Header separator row
   - A header-separator row is identified when a table-like line begins with the two-character sequence "|-". It typically consists of dashes and marks the boundary between header and data rows. Example:
     |----|-----|------|

   7.3 Data rows
   - A data row must start and end with '|' and contain the same number of cells as the header. Cells are delimited by unescaped '|' characters.
   - Each cell's textual content is trimmed of surrounding spaces and tabs when read.
   - Column count consistency is required: mismatched column counts are considered invalid for strict processing and writers enforce consistency when producing tables.

8. Escaping rules (table cell context)

   - The escape character '\\' is used to include special characters inside cell content.
   - Recognized escape sequences:
     - "\\|" => literal pipe character ('|')
     - "\\\\" => literal backslash ('\\')
     - "\\n" => newline character (writer emits "\\n"; environment-dependent NewLineEscaped may be "\\r\\n")
   - Separator detection: a '|' character is considered a column separator unless it is preceded by an odd number of consecutive backslashes. The reader locates the next unescaped '|' when extracting cells.
   - Unescaping: recognized escape sequences are translated to their corresponding characters; unknown escape sequences are preserved with the backslash left in place.

9. Writer semantics and configurable options

   - Section emission: the writer outputs section headers as "[name]" followed by a newline.
   - Property emission: properties are written as key=value. String values are typically output with surrounding double quotes; numeric and boolean types are formatted using CultureInfo.InvariantCulture.
   - Table emission: the writer emits header and data rows enclosed in '|' with a single space padding between the pipe and cell content. Writer produces a header-separator row composed of '-' characters.
   - Escaping: the writer escapes '|', '\\' and newline characters inside cell content using the documented escape sequences.
   - WriterOptions:
     - LeaveOpen: when true, disposing the writer does not dispose the underlying TextWriter/Stream.
     - EscapeQuotes: when true, the writer prefixes a quotation character with a backslash when writing quoted property values.
     - EscapeNewLineChars: when true, the writer writes newline sequences in escaped form (NewLineEscaped) instead of emitting a literal newline.

10. Examples

   - Section and properties:
     [CONTRACT.INFO]
     id = "AAA"
     source = "XXX"

   - Table:
     | id | name |
     |----|------|
     | 1  | Alice |
     | 2  | Bob   |

11. Implementation mapping

   - SectionHeaderReader: section header extraction logic.
   - IonProperty: property parsing and trimming semantics.
   - TableRowReader: cell splitting, escape detection and unescaping.
   - IonWriter: formatting, padding and escaping for emitted ION content.

This specification is authoritative for files produced and consumed by the Anixe.Ion components in this repository. Refer to the listed implementation files for algorithmic details and exact behavior.
