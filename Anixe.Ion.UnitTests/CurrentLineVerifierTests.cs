using System;
using Xunit;

namespace Anixe.Ion.UnitTests
{
    public class CurrentLineVerifierTests
    {
        private readonly CurrentLineVerifier target;

        public CurrentLineVerifierTests()
        {
            this.target = new CurrentLineVerifier();
        }

        [Theory]
        [InlineData("",        false)]
        [InlineData(" ",       false)]
        [InlineData("[NAME]",  true)]
        [InlineData("#[NAME]", false)]
        [InlineData("|date|",  false)]
        [InlineData("|-",      false)]
        [InlineData("|-1",     false)]
        [InlineData("abc",     false)]
        [InlineData(";abc",    false)]
        [InlineData("1abc",    false)]
        public void IsSectionHeader_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsSectionHeader(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        false)]
        [InlineData(" ",       false)]
        [InlineData("[NAME]",  false)]
        [InlineData("#[NAME]", false)]
        [InlineData("|date|",  false)]
        [InlineData("|-",      false)]
        [InlineData("|-1",     false)]
        [InlineData(" abc",    true)]
        [InlineData("abc",     true)]
        [InlineData(";abc",    true)]
        [InlineData("1abc",    true)]
        public void IsProperty_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsProperty(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        false)]
        [InlineData(" ",       false)]
        [InlineData("[NAME]",  false)]
        [InlineData("#[NAME]", true)]
        [InlineData("|date|",  false)]
        [InlineData("|-",      false)]
        [InlineData("|-1",     false)]
        [InlineData("abc",     false)]
        [InlineData(";abc",    false)]
        [InlineData("1abc",    false)]
        public void IsComment_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsComment(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        false)]
        [InlineData(" ",       false)]
        [InlineData("[NAME]",  false)]
        [InlineData("#[NAME]", false)]
        [InlineData("|date|",  true)]
        [InlineData("|-",      true)]
        [InlineData("|-1",     true)]
        [InlineData("abc",     false)]
        [InlineData(";abc",    false)]
        [InlineData("1abc",    false)]
        public void IsTableRow_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsTableRow(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        false, false)]
        [InlineData(" ",       false, false)]
        [InlineData("[NAME]",  false, false)]
        [InlineData("#[NAME]", false, false)]
        [InlineData("|date|",  false, true)]
        [InlineData("|-",      false, false)]
        [InlineData("|-1",     false, false)]
        [InlineData("abc",     false, false)]
        [InlineData(";abc",    false, false)]
        [InlineData("1abc",    false, false)]
        [InlineData("| x",     false, true)]
        [InlineData("| x",     true,  false)]
        [InlineData("|-",      true,  false)]
        public void IsTableHeaderRow_Tests(string currentLine, bool passedHeader, bool expectedResult)
        {
            var result = this.target.IsTableHeaderRow(new ArraySegment<char>(currentLine.ToCharArray()), passedHeader);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        true,  false)]
        [InlineData(" ",       true,  false)]
        [InlineData("[NAME]",  true,  false)]
        [InlineData("#[NAME]", true,  false)]
        [InlineData("|date|",  true,  true)]
        [InlineData("|-",      true,  false)]
        [InlineData("|-1",     true,  false)]
        [InlineData("abc",     true,  false)]
        [InlineData(";abc",    true,  false)]
        [InlineData("1abc",    true,  false)]
        [InlineData("| x",     false, false)]
        [InlineData("| x",     true,  true)]
        [InlineData("|-",      false, false)]
        public void IsTableDataRow_Tests(string currentLine, bool passedHeader, bool expectedResult)
        {
            var result = this.target.IsTableDataRow(new ArraySegment<char>(currentLine.ToCharArray()), passedHeader);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        false)]
        [InlineData(" ",       false)]
        [InlineData("[NAME]",  false)]
        [InlineData("#[NAME]", false)]
        [InlineData("|date|",  false)]
        [InlineData("|-",      true)]
        [InlineData("|-1",     true)]
        [InlineData("abc",     false)]
        [InlineData(";abc",    false)]
        [InlineData("1abc",    false)]
        public void IsTableHeaderSeparatorRow_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsTableHeaderSeparatorRow(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",        true)]
        [InlineData(" ",       true)]
        [InlineData("[NAME]",  false)]
        [InlineData("#[NAME]", false)]
        [InlineData("|date|",  false)]
        [InlineData("|-",      false)]
        [InlineData("|-1",     false)]
        [InlineData("abc",     false)]
        [InlineData(";abc",    false)]
        [InlineData("1abc",    false)]
        public void IsEmptyLine_Tests(string currentLine, bool expectedResult)
        {
            var result = this.target.IsEmptyLine(new ArraySegment<char>(currentLine.ToCharArray()));
            Assert.Equal(expectedResult, result);
        }
    }
}