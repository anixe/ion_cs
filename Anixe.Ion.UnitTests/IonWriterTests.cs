using Anixe.Ion.Exceptions;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Anixe.Ion.UnitTests
{
    internal class IonWriterTests
    {
        [Test]
        public void Should_Write_Section()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=\"value\"{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Escaped_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb), new WriterOptions { EscapeQuotes = true }))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=\\\"value\\\"{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Escaped_Newlines_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb), new WriterOptions { EscapeQuotes = true, EscapeNewLineChars = true }))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=\\\"value\\\"{0}", Consts.IonSpecialChars.NewLineEscaped), sb.ToString());
        }

        [Test]
        public void Should_Write_Char_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 'x');
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=\"x\"{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Integer_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 1000);
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=1000{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Double_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 25.22d);
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=25.22{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Decimal_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 25.22m);
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=25.22{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Float_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 24.3f);
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=24.3{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Custom_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", (tw) =>
                {
                    tw.Write('[');
                    tw.Write(23);
                    tw.Write(',');
                    tw.Write(23);
                    tw.Write(',');
                    tw.Write(23);
                    tw.Write(',');
                    tw.Write(23);
                    tw.Write(']');
                });
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=[23,23,23,23]{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Multiple_Properties()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", 24.3f);
                subject.WriteProperty("test1", "test1val");
                subject.WriteProperty("test2", true);
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.AreEqual(string.Format("[META]{0}test=24.3{0}test1=\"test1val\"{0}test2=true{0}", Environment.NewLine), sb.ToString());
        }

        [Test]
        public void Should_Write_Table_Header()
        {
            var header = new string[] { "key", "val" };
            var expected = new string[]
            {
                "[DATA]",
                "| key | val |",
                "|-----|-----|",
                string.Empty
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("DATA");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteTableHeader(header);
                Assert.AreEqual(WriterState.Section | WriterState.TableHeader, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Write_Table_Rows()
        {
            var row = new string[] { "unit_type", "miles" };
            var expected = new string[]
            {
                "[DATA]",
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("DATA");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.AreEqual(WriterState.Section | WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void IIonWriter_WriteTableCell_Throws_InvalidTableCellDataException()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableCell("\n"));
                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableCell("|"));

                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableCell("\n".ToCharArray(), 0, 1));
                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableCell("|".ToCharArray(), 0, 1));
            }
        }

        [Test]
        public void IIonWriter_WriteTableRow_Throws_InvalidTableCellDataException()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableRow(new[] { "\n" }));
                Assert.Throws<InvalidTableCellDataException>(() => subject.WriteTableRow(new[] { "|" }));
            }
        }

        [Test]
        public void Should_Write_Table_Outside_Section()
        {
            var row = new string[] { "unit_type", "miles" };
            var expected = new string[]
            {
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.AreEqual(WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Write_Table_Cell_With_Custom_Value()
        {
            var expected = new string[]
            {
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty,
                "| my custom table |",
                string.Empty,
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteTableCell((tw) => tw.Write("unit_type"));
                subject.WriteTableCell((tw) => tw.Write("miles"), true);
                subject.WriteTableCell("unit_type");
                subject.WriteTableCell("miles", true);
                Assert.AreEqual(WriterState.TableRow, subject.State);
                subject.WriteEmptyLine();
                Assert.AreEqual(WriterState.None, subject.State);
                subject.WriteTableCell((tw) => tw.Write("my custom table"), true);
                Assert.AreEqual(WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Write_Tables()
        {
            var row = new string[] { "unit_type", "miles" };
            var row2 = new string[] { "unit_type", "miles", "23" };
            var expected = new string[]
            {
                "[DATA]",
                string.Empty,
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty,
                "| unit_type | miles | 23 |",
                "| unit_type | miles | 23 |"
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("DATA");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.AreEqual(WriterState.Section | WriterState.TableRow, subject.State);
                subject.WriteEmptyLine();
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteTableRow(row2);
                subject.WriteTableRow(row2);
                Assert.AreEqual(WriterState.Section | WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Write_Whole_Table()
        {
            var header = new string[] { "key", "val" };
            var row = new string[] { "unit_type", "miles" };
            var expected = new string[]
            {
                "[DATA]",
                "| key | val |",
                "|-----|-----|",
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("DATA");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteTableHeader(header);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.AreEqual(WriterState.Section | WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Write_Multiple_Sections()
        {
            var header = new string[] { "key", "val" };
            var row = new string[] { "unit_type", "miles" };
            var expected = new string[]
            {
                "[META]",
                "test=\"value\"",
                string.Empty,
                "[DATA]",
                "table_name=\"test\"",
                string.Empty,
                "| key | val |",
                "|-----|-----|",
                "| unit_type | miles |",
                "| unit_type | miles |",
                string.Empty
            };
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.AreEqual(WriterState.Section | WriterState.Property, subject.State);
                subject.WriteSection("DATA");
                Assert.AreEqual(WriterState.Section, subject.State);
                subject.WriteProperty("table_name", "test");
                subject.WriteEmptyLine();
                subject.WriteTableHeader(header);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.AreEqual(WriterState.Section | WriterState.TableRow, subject.State);
            }
            CollectionAssert.AreEquivalent(expected, sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        [Test]
        public void Should_Validate_Section()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection(null!);
            }

            Assert.Throws<ArgumentNullException>(action, "Section name must be provided");
        }

        [Test]
        public void Should_Validate_Table()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteTableHeader(null!);
            }

            Assert.Throws<InvalidOperationException>(action, "Only section can be at the top of document");
        }

        [Test]
        public void Should_Validate_Table_Columns()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection("TEST");
                subject.WriteTableHeader(null!);
            }

            Assert.Throws<ArgumentNullException>(action, "Cannot create empty table header");
        }

        [Test]
        public void Should_Validate_Table_Headers()
        {
            static void action()
            {
                var header = new string[] { "key", "val" };

                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection("TEST");
                subject.WriteTableHeader(header);
                subject.WriteTableHeader(header);
            }

            Assert.Throws<InvalidOperationException>(action, "Table can have ony one header");
        }

        [Test]
        public void Should_Validate_Table_Row()
        {
            static void action()
            {
                var header = new string[] { "key", "val" };
                var row = new string[] { "key" };

                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection("TEST");
                subject.WriteTableHeader(header);
                subject.WriteTableRow(row);
            }

            Assert.Throws<ArgumentException>(action, "Must provide the same number of columns within the same table");
        }

        [Test]
        public void Should_Validate_Table_Rows()
        {
            static void action()
            {
                var row1 = new string[] { "key", "val" };
                var row2 = new string[] { "key" };

                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection("TEST");
                subject.WriteTableRow(row1);
                subject.WriteTableRow(row2);
            }

            Assert.Throws<ArgumentException>(action, "Must provide the same number of columns within the same table");
        }
    }
}
