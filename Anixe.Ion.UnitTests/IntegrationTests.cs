using System;
using System.Collections.Generic;
using Xunit;

namespace Anixe.Ion.UnitTests
{
    public sealed class IntegrationTests : IDisposable
    {
        private readonly IIonReader target;

        public IntegrationTests()
        {
            this.target = IonReaderFactory.Create(FileLoader.GetExamplesIonPath());
        }

        public void Dispose()
        {
            this.target.Dispose();
        }

        [Fact]
        public void Verifies_Each_Ion_File_Line()
        {
            var assertions = new List<Action>
            {
                IsSectionHeader_InFirstLine,
                IsProperty_InSecondLine,
                IsProperty_InThirdLine,
                IsEmptyLine_InFourthLine,
                IsProperty_InFifthLine,
                IsEmptyLine_InSixthLine,
                IsSectionHeader_InSeventhLine,
                IsTableRow_InEighthLine,
                IsTableHeaderSeparatorRow_InNinethLine,
                IsTableRow_InTenthLine,
                IsTableRow_InEleventhLine,
                IsEmptyLine_InTwelfthLine,
                IsEmptyLine_InThirteenthLine,
                IsEmptyLine_InFourteenthLine,
                IsSectionHeader_InFiftheenthLine,
                IsTableRow_InSixteenthLine,
                IsTableHeaderSeparatorRow_InSeventeenthLine,
                IsTableRow_InEighteenthLine,
            };

            while (target.Read())
            {
                assertions[this.target.CurrentLineNumber - 1].Invoke();
            }
        }

        private void IsSectionHeader_InFirstLine()
        {
            Assert.True(this.target.IsSectionHeader);
            Assert.Equal("[CONTRACT.INFO]", this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(1, this.target.CurrentLineNumber);
        }

        private void IsProperty_InSecondLine()
        {
            Assert.True(this.target.IsProperty);
            Assert.Equal("id = \"AAA\"", this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(2, this.target.CurrentLineNumber);
        }

        private void IsProperty_InThirdLine()
        {
            Assert.True(this.target.IsProperty);
            Assert.Equal("source = \"XXX\"", this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(3, this.target.CurrentLineNumber);
        }

        private void IsEmptyLine_InFourthLine()
        {
            Assert.True(this.target.IsEmptyLine);
            Assert.Equal(string.Empty, this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(4, this.target.CurrentLineNumber);
        }

        private void IsProperty_InFifthLine()
        {
            Assert.True(this.target.IsProperty);
            Assert.Equal("city = \"YYY\"", this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(5, this.target.CurrentLineNumber);
        }

        private void IsEmptyLine_InSixthLine()
        {
            Assert.True(this.target.IsEmptyLine);
            Assert.Equal(string.Empty, this.target.CurrentLine);
            Assert.Equal("CONTRACT.INFO", this.target.CurrentSection);
            Assert.Equal(6, this.target.CurrentLineNumber);
        }

        private void IsSectionHeader_InSeventhLine()
        {
            Assert.True(this.target.IsSectionHeader);
            Assert.Equal("[DEF.SECTION]", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(7, this.target.CurrentLineNumber);
        }

        private void IsTableRow_InEighthLine()
        {
            Assert.True(this.target.IsTableRow);
            Assert.True(this.target.IsTableHeaderRow);
            Assert.Equal("|  id | 1st_column_description | 2nd_column_description |", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(8, this.target.CurrentLineNumber);
        }

        private void IsTableHeaderSeparatorRow_InNinethLine()
        {
            Assert.True(this.target.IsTableHeaderSeparatorRow);
            Assert.False(this.target.IsTableHeaderRow);
            Assert.Equal("|-----|------------------------|------------------------|", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(9, this.target.CurrentLineNumber);
        }

        private void IsTableRow_InTenthLine()
        {
            Assert.True(this.target.IsTableRow);
            Assert.True(this.target.IsTableDataRow);
            Assert.Equal("| 123 | ZZZ                    | VVV                    |", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(10, this.target.CurrentLineNumber);
        }

        private void IsTableRow_InEleventhLine()
        {
            Assert.True(this.target.IsTableRow);
            Assert.Equal("| abc | XXX                    | UUU                    |", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(11, this.target.CurrentLineNumber);
        }

        private void IsEmptyLine_InTwelfthLine()
        {
            Assert.True(this.target.IsEmptyLine);
            Assert.Equal(string.Empty, this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(12, this.target.CurrentLineNumber);
        }

        private void IsEmptyLine_InThirteenthLine()
        {
            Assert.True(this.target.IsEmptyLine);
            Assert.Equal(string.Empty, this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(13, this.target.CurrentLineNumber);
        }

        private void IsEmptyLine_InFourteenthLine()
        {
            Assert.True(this.target.IsEmptyLine);
            Assert.Equal(string.Empty, this.target.CurrentLine);
            Assert.Equal("DEF.SECTION", this.target.CurrentSection);
            Assert.Equal(14, this.target.CurrentLineNumber);
        }

        private void IsTableRow_InEighteenthLine()
        {
            Assert.True(this.target.IsTableRow);
            Assert.Equal("| 123 | ZZZ                    | VVV                    |", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION2", this.target.CurrentSection);
            Assert.Equal(18, this.target.CurrentLineNumber);
        }

        private void IsTableHeaderSeparatorRow_InSeventeenthLine()
        {
            Assert.True(this.target.IsTableHeaderSeparatorRow);
            Assert.False(this.target.IsTableHeaderRow);
            Assert.Equal("|-----|------------------------|------------------------|", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION2", this.target.CurrentSection);
            Assert.Equal(17, this.target.CurrentLineNumber);
        }

        private void IsTableRow_InSixteenthLine()
        {
            Assert.True(this.target.IsTableRow);
            Assert.Equal("|  id | 1st_column_description | 2nd_column_description |", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION2", this.target.CurrentSection);
            Assert.Equal(16, this.target.CurrentLineNumber);
        }

        private void IsSectionHeader_InFiftheenthLine()
        {
            Assert.True(this.target.IsSectionHeader);
            Assert.Equal("[DEF.SECTION2]", this.target.CurrentLine);
            Assert.Equal("DEF.SECTION2", this.target.CurrentSection);
            Assert.Equal(15, this.target.CurrentLineNumber);
        }
    }
}
