using NUnit.Framework;
using System;
using System.Collections.Generic;
using static NUnit.StaticExpect.Expectations;

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
            while (target.Read())
            {
                this.assertions[this.target.CurrentLineNumber - 1].Invoke();
            }
        }

        #region Assertions

        private void IsSectionHeader_InFirstLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine, Is.EqualTo("[CONTRACT.INFO]"));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(1));
        }

        private void IsProperty_InSecondLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine, Is.EqualTo("id = \"AAA\""));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(2));
        }

        private void IsProperty_InThirdLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine, Is.EqualTo("source = \"XXX\""));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(3));
        }

        private void IsEmptyLine_InFourthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine, Is.EqualTo(string.Empty));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(4));
        }

        private void IsProperty_InFifthLine()
        {
            Expect(this.target.IsProperty);
            Expect(this.target.CurrentLine, Is.EqualTo("city = \"YYY\""));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(5));
        }

        private void IsEmptyLine_InSixthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine, Is.EqualTo(string.Empty));
            Expect(this.target.CurrentSection, Is.EqualTo("CONTRACT.INFO"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(6));
        }

        private void IsSectionHeader_InSeventhLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine, Is.EqualTo("[DEF.SECTION]"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(7));
        }

        private void IsTableRow_InEighthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.IsTableHeaderRow);
            Expect(this.target.CurrentLine, Is.EqualTo("|  id | 1st_column_description | 2nd_column_description |"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(8));
        }

        private void IsTableHeaderSeparatorRow_InNinethLine()
        {
            Expect(this.target.IsTableHeaderSeparatorRow);
            Expect(this.target.IsTableHeaderRow, Is.False);
            Expect(this.target.CurrentLine, Is.EqualTo("|-----|------------------------|------------------------|"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(9));
        }

        private void IsTableRow_InTenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.IsTableDataRow);
            Expect(this.target.CurrentLine, Is.EqualTo("| 123 | ZZZ                    | VVV                    |"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(10));
        }

        private void IsTableRow_InEleventhLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine, Is.EqualTo("| abc | XXX                    | UUU                    |"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(11));
        }

        private void IsEmptyLine_InTwelfthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine, Is.EqualTo(string.Empty));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(12));
        }

        private void IsEmptyLine_InThirteenthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine, Is.EqualTo(string.Empty));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(13));
        }

        private void IsEmptyLine_InFourteenthLine()
        {
            Expect(this.target.IsEmptyLine);
            Expect(this.target.CurrentLine, Is.EqualTo(string.Empty));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(14));
        }

        private void IsTableRow_InEighteenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine, Is.EqualTo("| 123 | ZZZ                    | VVV                    |"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION2"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(18));
        }

        private void IsTableHeaderSeparatorRow_InSeventeenthLine()
        {
            Expect(this.target.IsTableHeaderSeparatorRow);
            Expect(this.target.IsTableHeaderRow, Is.False);
            Expect(this.target.CurrentLine, Is.EqualTo("|-----|------------------------|------------------------|"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION2"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(17));
        }

        private void IsTableRow_InSixteenthLine()
        {
            Expect(this.target.IsTableRow);
            Expect(this.target.CurrentLine, Is.EqualTo("|  id | 1st_column_description | 2nd_column_description |"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION2"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(16));
        }

        private void IsSectionHeader_InFiftheenthLine()
        {
            Expect(this.target.IsSectionHeader);
            Expect(this.target.CurrentLine, Is.EqualTo("[DEF.SECTION2]"));
            Expect(this.target.CurrentSection, Is.EqualTo("DEF.SECTION2"));
            Expect(this.target.CurrentLineNumber, Is.EqualTo(15));
        }

        #endregion
    }
}

