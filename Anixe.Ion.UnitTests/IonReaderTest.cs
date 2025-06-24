using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests
{
    public class IonReaderTest
    {
        [Fact]
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
                        Assert.Equal(12, columns.Length);
                        if (counter == 1)
                        {
                            Assert.Equal("Berlin Mitte Kronenstraße", columns[0].Trim());
                        }
                        if (counter == 2)
                        {
                            Assert.Equal("DANS PARKING AUTOCIT®Õ NIVEAU -2", columns[0].Trim());
                        }
                    }
                }
            }
            Assert.Equal(3, counter);
        }

        [Fact]
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
            Assert.Equal(8, props.Count);
            Assert.Equal("currency = \"EUR\"", props[0]);
            Assert.Equal("language = \"de\"", props[1]);
            Assert.Equal("sales_market = \"DE\"", props[2]);
            Assert.Equal("timeout = 5000", props[3]);
            Assert.Equal("sipp = \"ECAR\"", props[4]);
            Assert.Equal("price_margin = %5", props[5]);
            Assert.Equal("disabled_log_urls = [ \"/G\", \"/cars\", \"/cars_group_by\" ]", props[6]);
            Assert.Equal("enabled_log_urls = [ ]", props[7]);
        }

        [Fact]
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
                        Assert.Equal(12, columns.Length);
                        if (counter == 1)
                        {
                            Assert.Equal("Berlin Mitte Kronenstraße", columns[0].Trim());
                        }
                        if (counter == 2)
                        {
                            Assert.Equal("DANS PARKING AUTOCIT®Õ NIVEAU -2", columns[0].Trim());
                        }
                    }
                }
            }
            Assert.Equal(3, counter);
        }

        [Fact]
        public void CurrentSection_Returns_Null_Before_First_Read()
        {
            using var reader = IonReaderFactory.Create(Stream.Null);
            Assert.Null(reader.CurrentSection);
            reader.Read();
            Assert.NotNull(reader.CurrentSection);
        }

        [Fact]
        public void Should_Read_Table_Cell()
        {
            //Assert.Equal("", ReadTableCell(""));
            //Assert.Equal("x", ReadTableCell("x"));
            //Assert.Equal("\\a", ReadTableCell("\\a"));
            //Assert.Equal("\n", ReadTableCell("\\n"));
            Assert.Equal("|", ReadTableCell("\\|"));
            //Assert.Equal("Berlin Mitte Kronenstraße", ReadTableCell("Berlin Mitte Kronenstraße"));
            static string ReadTableCell(string ionCellContent)
            {
                var ion = $"""
                    [TABLE]
                    | Column |
                    |--------|
                    | {ionCellContent} |
                    """;

                var ms = new MemoryStream(Encoding.UTF8.GetBytes(ion));
                using (var reader = IonReaderFactory.Create(ms))
                {
                    while (reader.Read())
                    {
                        if (reader.IsTableDataRow)
                        {
                            var rowReader = reader.ReadTableRow();
                            return rowReader.ReadNext().ToString();
                        }
                    }
                }

                throw new InvalidOperationException("No table data row found.");
            }
        }

        [Fact]
        public void ReadTableCellTest()
        {
            var ion = """
                [TABLE]
                | col1 | col2 | col3 | col4|
                |--------|-|-----|----|
                | a | b | \ntext\| separated | |
                """;

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(ion));
            using (var reader = IonReaderFactory.Create(ms))
            {
                while (reader.Read())
                {
                    if (reader.IsTableDataRow)
                    {
                        var rowReader = reader.ReadTableRow();
                        Assert.Equal("a", rowReader.ReadNext().ToString());
                        Assert.Equal("b", rowReader.ReadNext().ToString());
                        Assert.Equal("\ntext| separated", rowReader.ReadNext().ToString());
                        Assert.Equal("", rowReader.ReadNext().ToString());
                        try
                        {
                            rowReader.ReadNext();
                            Assert.Fail("Should not be reached because an exception should be thrown.");
                        }
                        catch (InvalidOperationException)
                        {
                            // behavior is correct, no more cells should be available
                        }

                        return;
                    }
                }
            }

            throw new InvalidOperationException("No table data row found.");
        }
    }

}