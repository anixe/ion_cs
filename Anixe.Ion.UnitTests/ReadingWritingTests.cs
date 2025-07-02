using System.IO;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests;

public sealed class ReadingWritingTests
{
    [Theory]
    [InlineData("")]
    [InlineData("|")]
    [InlineData("Value")]
    [InlineData("\\")]
    [InlineData("\n")]
    [InlineData("\\n")]
    [InlineData("\\\n")]
    [InlineData("\\\\n")]
    [InlineData("\\|")]
    [InlineData("\\\\|")]
    [InlineData("\\x")]
    public void ReadAndWriteTableCellValues(string cellValue)
    {
        // write
        var sb = new StringBuilder();
        using var tw = new StringWriter(sb);
        using (var writer = new IonWriter(tw))
        {
            writer.WriteTableRow(cellValue);
        }

        var ion = sb.ToString();

        // read
        string? actualCellValue = null;

        var reader = IonReaderFactory.Create(new MemoryStream(Encoding.UTF8.GetBytes(ion)));
        while (reader.Read())
        {
            if (reader.IsTableRow)
            {
                var rowReader = reader.ReadTableRow();
                actualCellValue = rowReader.ReadNextString();
            }
        }

        Assert.Equal(cellValue, actualCellValue);
    }
}