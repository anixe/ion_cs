using System;
using Xunit;

namespace Anixe.Ion.UnitTests;

public class TableRowReaderTests
{
    public static TheoryData<string, string[]> GetTableRowReaderData() => new()
    {
        { "| Berlin Mitte Kronenstraße |", ["Berlin Mitte Kronenstraße"] },
        { "|DANS PARKING AUTOCIT®Õ NIVEAU -2 |", ["DANS PARKING AUTOCIT®Õ NIVEAU -2"] },
        { "|Column1|Column2|Column3|", ["Column1", "Column2", "Column3"] },
        { "|Value1\\|Escaped|Value2\\nNewLine|", ["Value1|Escaped", "Value2\nNewLine"] },
        { "|SingleValue|", ["SingleValue"] },
        { "|ValueWith\\|Multiple\\|Escapes|", ["ValueWith|Multiple|Escapes"] },
        { "|ValueWith\\nNewLine|", ["ValueWith\nNewLine"] },
        { "||", [""] },
        { "| |", [""] },
        { "|a|", ["a"] },
        { "|a|\n", ["a"] },
        { "| a |", ["a"] },
        { "| \\\\ |", ["\\\\"] },
        { "| \\|\\ | |", ["|\\", ""] },
        { "| \\|\\ ||", ["|\\", ""] },
        { "| \\n |", ["\n"] },
        { "| \\\\n |", ["\\\n"] },
        { "|\t a\t |", ["a"] },
    };

    [Theory]
    [MemberData(nameof(GetTableRowReaderData))]
    public void ReadNextSpan_ShouldReturnExpectedValues(string input, string[] expectedValues)
    {
        var reader = new TableRowReader(input.AsSpan());
        foreach (var expectedValue in expectedValues)
        {
            var actualValue = reader.ReadNextSpan();
            Assert.Equal(expectedValue, actualValue);
        }

        try
        {
            reader.ReadNextSpan();
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException)
        {
            // Expected exception, do nothing
        }
    }

    [Theory]
    [InlineData("#|x|")]
    [InlineData("|x")]
    [InlineData("x|")]
    [InlineData("")]
    [InlineData("[XXX]")]
    public void Constructor_ShouldThrow_WhenRowHasIncorrectFormat(string rowLine)
    {
        try
        {
            new TableRowReader(rowLine);
            Assert.Fail("Expected FormatException was not thrown.");
        }
        catch (FormatException)
        {
            // Expected exception, do nothing
        }
    }

    [Fact]
    public void CanReadNext_ShouldReturnFalse_WhenNoMoreCellsToRead()
    {
        var reader = new TableRowReader("|Value1|Value2|");

        // read Value1
        Assert.True(reader.CanReadNext);
        reader.ReadNextSpan();

        // read Value2
        Assert.True(reader.CanReadNext);
        reader.ReadNextSpan();

        // no more cells to read
        Assert.False(reader.CanReadNext);
    }

    [Theory]
    [InlineData("||")]
    [InlineData("| |")]
    [InlineData("|\t|")]
    [InlineData("|Value1|")]
    public void CanReadNext_ShouldReturnTrue_ForCellsWithProvidedContent(string input)
    {
        var reader = new TableRowReader(input);
        Assert.True(reader.CanReadNext);
    }
}
