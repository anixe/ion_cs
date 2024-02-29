using System;
using NUnit.Framework;
using System.Collections.Generic;
using static NExpect.Expectations;
using NExpect;

namespace Anixe.Ion.UnitTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private IIonReader target = null!;
        private List<Action> assertions = null!;

        [OneTimeSetUp]
        public void Before_All_Test()
        {
            this.target = IonReaderFactory.Create(FileLoader.GetExamplesIonPath());

            this.assertions = new List<Action>
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
        }

        [OneTimeTearDown]
        public void After_All_Test()
        {
            this.target.Dispose();
        }

        [Test]
        public void Verifies_Each_Ion_File_Line()
        {
            while(target.Read())
            {
                this.assertions[this.target.CurrentLineNumber - 1].Invoke();
            }
        }

        #region Assertions

        private void IsSectionHeader_InFirstLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine).To.Equal("[CONTRACT.INFO]");
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(1);
        }

        private void IsProperty_InSecondLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine).To.Equal("id = \"AAA\"");
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(2);
        }

        private void IsProperty_InThirdLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine).To.Equal("source = \"XXX\"");
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(3);
        }

        private void IsEmptyLine_InFourthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine).To.Equal(string.Empty);
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(4);
        }

        private void IsProperty_InFifthLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine).To.Equal("city = \"YYY\"");
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(5);
        }

        private void IsEmptyLine_InSixthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine).To.Equal(string.Empty);
            Expect(this.target.CurrentSection).To.Equal("CONTRACT.INFO");
            Expect(this.target.CurrentLineNumber).To.Equal(6);
        }

        private void IsSectionHeader_InSeventhLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine).To.Equal("[DEF.SECTION]");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(7);
        }

        private void IsTableRow_InEighthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.IsTableHeaderRow);
            Expect(this.target.CurrentLine).To.Equal("|  id | 1st_column_description | 2nd_column_description |");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(8);
        }

        private void IsTableHeaderSeparatorRow_InNinethLine()
        {
            Expect(this.target.IsTableHeaderSeparatorRow);
            Expect(this.target.IsTableHeaderRow).To.Be.False();
            Expect(this.target.CurrentLine).To.Equal("|-----|------------------------|------------------------|");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(9);
        }

        private void IsTableRow_InTenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.IsTableDataRow);
            Expect(this.target.CurrentLine).To.Equal("| 123 | ZZZ                    | VVV                    |");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(10);
        }

        private void IsTableRow_InEleventhLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine).To.Equal("| abc | XXX                    | UUU                    |");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(11);
        }

        private void IsEmptyLine_InTwelfthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine).To.Equal(string.Empty);
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(12);
        }

        private void IsEmptyLine_InThirteenthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine).To.Equal(string.Empty);
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(13);
        }

        private void IsEmptyLine_InFourteenthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine).To.Equal(string.Empty);
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION");
            Expect(this.target.CurrentLineNumber).To.Equal(14);
        }

        private void IsTableRow_InEighteenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine).To.Equal("| 123 | ZZZ                    | VVV                    |");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION2");
            Expect(this.target.CurrentLineNumber).To.Equal(18);
        }

        private void IsTableHeaderSeparatorRow_InSeventeenthLine()
        {
            Expect(this.target.IsTableHeaderSeparatorRow);
            Expect(this.target.IsTableHeaderRow).To.Be.False();
            Expect(this.target.CurrentLine).To.Equal("|-----|------------------------|------------------------|");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION2");
            Expect(this.target.CurrentLineNumber).To.Equal(17);
        }

        private void IsTableRow_InSixteenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine).To.Equal("|  id | 1st_column_description | 2nd_column_description |");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION2");
            Expect(this.target.CurrentLineNumber).To.Equal(16);
        }

        private void IsSectionHeader_InFiftheenthLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine).To.Equal("[DEF.SECTION2]");
            Expect(this.target.CurrentSection).To.Equal("DEF.SECTION2");
            Expect(this.target.CurrentLineNumber).To.Equal(15);
        }

        #endregion
    }
}

