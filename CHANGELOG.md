# Anixe.Ion CHANGELOG

## 3.0.0
- Drop support of .NET below .NET 9
- Add method ReadTableRow() that returns reader for row cells and unescapes them if needed
- Change behavior of WriteTableRow() that previously thrown exception for '|' and newline charcter. Now it escapes them with \| and \n.
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
