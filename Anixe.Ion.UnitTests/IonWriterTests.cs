using Anixe.Ion.Exceptions;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests
{
    public class IonWriterTests
    {
        [Fact]
        public void Should_Write_Section()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=\"value\"{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Escaped_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb), new WriterOptions { EscapeQuotes = true }))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=\\\"value\\\"{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Escaped_Newlines_String_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb), new WriterOptions { EscapeQuotes = true, EscapeNewLineChars = true }))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=\\\"value\\\"{0}", Consts.IonSpecialChars.NewLineEscaped), sb.ToString());
        }

        [Fact]
        public void Should_Write_Char_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 'x');
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=\"x\"{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Integer_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 1000);
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=1000{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Double_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 25.22d);
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=25.22{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Decimal_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 25.22m);
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=25.22{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Float_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 24.3f);
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=24.3{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Custom_Property()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
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
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=[23,23,23,23]{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
        public void Should_Write_Multiple_Properties()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteSection("META");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", 24.3f);
                subject.WriteProperty("test1", "test1val");
                subject.WriteProperty("test2", true);
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
            }
            Assert.Equal(string.Format("[META]{0}test=24.3{0}test1=\"test1val\"{0}test2=true{0}", Environment.NewLine), sb.ToString());
        }

        [Fact]
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
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteTableHeader(header);
                Assert.Equal(WriterState.Section | WriterState.TableHeader, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
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
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.Equal(WriterState.Section | WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
        public void IIonWriter_WriteTableCell_Escapes()
        {
            Assert.Equal("""| \n |""", WriteTableCell(tw => tw.WriteTableCell("\n")));
            Assert.Equal("""| \| |""", WriteTableCell(tw => tw.WriteTableCell("|")));
            Assert.Equal("""| \n |""", WriteTableCell(tw => tw.WriteTableCell('\n')));
            Assert.Equal("""| \| |""", WriteTableCell(tw => tw.WriteTableCell('|')));
            Assert.Equal("""| \n |""", WriteTableCell(tw => tw.WriteTableCell("\n".ToCharArray(), 0, 1)));
            Assert.Equal("""| \| |""", WriteTableCell(tw => tw.WriteTableCell("|".ToCharArray(), 0, 1)));
            Assert.Equal("""| \n |""", WriteTableCell(tw => tw.WriteTableCell("\n".AsSpan())));
            Assert.Equal("""| \| |""", WriteTableCell(tw => tw.WriteTableCell("|".AsSpan())));
            Assert.Equal("""| \| |""", WriteTableCell(tw => tw.WriteTableCell(_tw => _tw.Write("|"))));
            Assert.Equal("""| \n |""", WriteTableCell(tw => tw.WriteTableCell(_tw => _tw.Write("\n"))));

            // escaped escape character does not escape the next character
            Assert.Equal("""| \\\| |""", WriteTableCell(tw => tw.WriteTableCell("\\|")));
            Assert.Equal("""| \\n |""", WriteTableCell(tw => tw.WriteTableCell("\\n")));
            Assert.Equal("""| \\\\n |""", WriteTableCell(tw => tw.WriteTableCell("\\\\n")));

            static string WriteTableCell(Action<IIonWriter> action)
            {
                var sb = new StringBuilder();
                using (var subject = new IonWriter(new StringWriter(sb)))
                {
                    action(subject);
                }
                return sb.ToString();
            }
        }

        [Fact]
        public void IIonWriter_WriteTableCell()
        {
            // string overload
            Assert.Equal("| x |", WriteTableCell(tw => tw.WriteTableCell("x")));

            // bool overload
            Assert.Equal("| True |", WriteTableCell(tw => tw.WriteTableCell(true)));

            // int overload
            Assert.Equal("| 1 |", WriteTableCell(tw => tw.WriteTableCell(1)));

            // long overload
            Assert.Equal("| 1 |", WriteTableCell(tw => tw.WriteTableCell(1L)));

            // double overload
            Assert.Equal("| 1.1 |", WriteTableCell(tw => tw.WriteTableCell(1.1)));
            Assert.Equal("| 1 |", WriteTableCell(tw => tw.WriteTableCell(1.0)));

            // decimal overload
            Assert.Equal("| 1.1 |", WriteTableCell(tw => tw.WriteTableCell(1.1m)));
            Assert.Equal("| 1.0 |", WriteTableCell(tw => tw.WriteTableCell(1.0m)));

            static string WriteTableCell(Action<IIonWriter> action)
            {
                var sb = new StringBuilder();
                using (var subject = new IonWriter(new StringWriter(sb)))
                {
                    action(subject);
                }
                return sb.ToString();
            }
        }

        [Fact]
        public void IIonWriter_WriteTableRow_Escapes()
        {
            var sb = new StringBuilder();
            using (var subject = new IonWriter(new StringWriter(sb)))
            {
                subject.WriteTableRow("\n", "|", "other");
            }
            Assert.Equal("""| \n | \| | other |""", sb.ToString().TrimEnd());
        }

        [Fact]
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
                Assert.Equal(WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
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
                Assert.Equal(WriterState.TableRow, subject.State);
                subject.WriteEmptyLine();
                Assert.Equal(WriterState.None, subject.State);
                subject.WriteTableCell((tw) => tw.Write("my custom table"), true);
                Assert.Equal(WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
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
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.Equal(WriterState.Section | WriterState.TableRow, subject.State);
                subject.WriteEmptyLine();
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteTableRow(row2);
                subject.WriteTableRow(row2);
                Assert.Equal(WriterState.Section | WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
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
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteTableHeader(header);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.Equal(WriterState.Section | WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
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
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("test", "value");
                Assert.Equal(WriterState.Section | WriterState.Property, subject.State);
                subject.WriteSection("DATA");
                Assert.Equal(WriterState.Section, subject.State);
                subject.WriteProperty("table_name", "test");
                subject.WriteEmptyLine();
                subject.WriteTableHeader(header);
                subject.WriteTableRow(row);
                subject.WriteTableRow(row);
                Assert.Equal(WriterState.Section | WriterState.TableRow, subject.State);
            }
            Assert.Equivalent(expected, sb.ToString().Split(Environment.NewLine, StringSplitOptions.None));
        }

        [Fact]
        public void Should_Validate_Section()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection(null!);
            }

            Assert.Throws<ArgumentNullException>("name", action);
        }

        [Fact]
        public void Should_Validate_Table()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteTableHeader([]);
            }

            var ex = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal("Only section can be at the top of document", ex.Message);
        }

        [Fact]
        public void Should_Validate_Table_Columns()
        {
            static void action()
            {
                var sb = new StringBuilder();
                using var subject = new IonWriter(new StringWriter(sb));
                subject.WriteSection("TEST");
                subject.WriteTableHeader(null!);
            }

            var ex = Assert.Throws<ArgumentNullException>("columns", action);
        }

        [Fact]
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

            var ex = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal("Table can have only one header", ex.Message);
        }

        [Fact]
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

            var ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Must provide the same number of columns within the same table (Parameter 'columns')", ex.Message);
        }

        [Fact]
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

            var ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Must provide the same number of columns within the same table (Parameter 'columns')", ex.Message);
        }
    }
}
