using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
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
        public void Should_Read_Ion_With_Windows_Line_Endings()
        {
            var props = new List<string>();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileLoader.GetIonWithWindowsLineEndings())))
            using (var reader = IonReaderFactory.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.CurrentSection != "DEFAULTS")
                    {
                        continue;
                    }
                    if (reader.IsEmptyLine)
                    {
                        break;
                    }
                    if (reader.IsProperty)
                    {
                        props.Add(reader.CurrentLine);
                    }
                }
            }
            Assert.AreEqual(8, props.Count);
            Assert.AreEqual("currency = \"EUR\"", props[0]);
            Assert.AreEqual("language = \"de\"", props[1]);
            Assert.AreEqual("sales_market = \"DE\"", props[2]);
            Assert.AreEqual("timeout = 5000", props[3]);
            Assert.AreEqual("sipp = \"ECAR\"", props[4]);
            Assert.AreEqual("price_margin = %5", props[5]);
            Assert.AreEqual("disabled_log_urls = [ \"/G\", \"/cars\", \"/cars_group_by\" ]", props[6]);
            Assert.AreEqual("enabled_log_urls = [ ]", props[7]);
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

        [Test]
        public void CurrentSection_Returns_Null_Before_First_Read()
        {
            using var reader = IonReaderFactory.Create(Stream.Null);
            Assert.Null(reader.CurrentSection);
            reader.Read();
            Assert.NotNull(reader.CurrentSection);
        }
    }
}