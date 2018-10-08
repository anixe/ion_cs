using System;
using System.IO;
using System.IO.Compression;
using NUnit.Framework;

namespace Anixe.Ion.UnitTests
{
    [TestFixture]
    public class IonReaderTest
    {
        [Test]
        public void Should_Read_Ins_Ion()
        {
            int counter = 0;
            using (var stream = File.OpenRead(FileLoader.GetInsIonPath()))
            using (var reader = IonReaderFactory.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.CurrentSection != "INSURANCE")
                    {
                        continue;
                    }
                    if (reader.IsEmptyLine)
                    {
                        break;
                    }
                    if (reader.IsTableDataRow)
                    {
                        counter++;
                        var columns = reader.CurrentLine.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        Assert.AreEqual(12, columns.Length);
                        if (counter == 1)
                        {
                            Assert.AreEqual("Berlin Mitte Kronenstraße", columns[0].Trim());
                        }
                        if (counter == 2)
                        {
                            Assert.AreEqual("DANS PARKING AUTOCIT®Õ NIVEAU -2", columns[0].Trim());
                        }
                    }
                }
            }
            Assert.AreEqual(3, counter);
        }


        [Test]
        public void Should_Read_Gzipped_Ins_Ion()
        {
            int counter = 0;
            using (var stream = File.OpenRead(FileLoader.GetInsGzIonPath()))
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = IonReaderFactory.Create(gzip))
            {
                while (reader.Read())
                {
                    if (reader.CurrentSection != "INSURANCE")
                    {
                        continue;
                    }
                    if (reader.IsEmptyLine)
                    {
                        break;
                    }
                    if (reader.IsTableDataRow)
                    {
                        counter++;
                        var columns = reader.CurrentLine.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        Assert.AreEqual(12, columns.Length);
                        if (counter == 1)
                        {
                            Assert.AreEqual("Berlin Mitte Kronenstraße", columns[0].Trim());
                        }
                        if (counter == 2)
                        {
                            Assert.AreEqual("DANS PARKING AUTOCIT®Õ NIVEAU -2", columns[0].Trim());
                        }
                    }
                }
            }
            Assert.AreEqual(3, counter);
        }
        
    }
}