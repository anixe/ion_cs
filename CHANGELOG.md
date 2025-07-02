# Anixe.Ion CHANGELOG

## 3.0.2
- Fix escaping in some cases; now all "\" in table cells are escaped with "\"
- Fix WriteTableCell was not escaping when TextWriter action was provided

## 3.0.1
- Rename TableRowReader.ReadNext() to ReadNextSpan()
- Add ReadNextString() method to TableRowReader
- Trim tab characters when reading table row cells
- Use always InvariantCulture for writing numbers into table cells
- Add property CanReadNext to TableRowReader to check if there are more cells in the row to be read

## 3.0.0
- Drop support of .NET below .NET 9
- Add method ReadTableRow() that returns reader for row cells and unescapes them if needed
- Change behavior of WriteTableRow() that previously thrown exception for '|' and newline character. Now it escapes them with \| and \n.
- Add metadata to the Nuget package
- various small performance improvements
- add missing nullable annotations MemberNotNullWhen
- change some exceptions to use standard .NET messages instead of custom ones
- change testing library from NUnit to xUnit

## 2.1.1
- added build for .NET Standard 2.0

## 2.1.0
- added IonProperty ref-struct that represents property row of IIonReader
- added extension method IIonReader.ReadProperty() that returns IonProperty struct
- ION reading performance optimization
