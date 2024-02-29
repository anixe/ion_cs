# Anixe.Ion CHANGELOG

## 2.1.2
- added build for .NET 8
- mark IonReader.CurrentSection as not null if IsSectionHeader was checked before
 
## 2.1.1
- added build for .NET Standard 2.0

## 2.1.0
- added IonProperty ref-struct that represents property row of IIonReader
- added extension method IIonReader.ReadProperty() that returns IonProperty struct
- ION reading performance optimization
