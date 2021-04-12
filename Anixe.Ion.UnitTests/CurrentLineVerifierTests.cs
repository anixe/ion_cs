using NUnit.Framework;

namespace Anixe.Ion.UnitTests
{
    internal class CurrentLineVerifierTests
    {
        private CurrentLineVerifier target = null!;

        [SetUp]
        public void Before_Each_Test()
        {
            this.target = new CurrentLineVerifier();
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = true)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = false)]
        [TestCase("|-1",     ExpectedResult = false)]
        [TestCase("abc",     ExpectedResult = false)]
        [TestCase(";abc",    ExpectedResult = false)]
        [TestCase("1abc",    ExpectedResult = false)]
        public bool IsSectionHeader_Tests(string currentLine)
        {
            return this.target.IsSectionHeader(currentLine.ToCharArray());
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = false)]
        [TestCase("|-1",     ExpectedResult = false)]
        [TestCase(" abc",     ExpectedResult = true)]
        [TestCase("abc",     ExpectedResult = true)]
        [TestCase(";abc",    ExpectedResult = true)]
        [TestCase("1abc",    ExpectedResult = true)]
        public bool IsProperty_Tests(string currentLine)
        {
            return this.target.IsProperty(currentLine.ToCharArray());
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = true)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = false)]
        [TestCase("|-1",     ExpectedResult = false)]
        [TestCase("abc",     ExpectedResult = false)]
        [TestCase(";abc",    ExpectedResult = false)]
        [TestCase("1abc",    ExpectedResult = false)]
        public bool IsComment_Tests(string currentLine)
        {
            return this.target.IsComment(currentLine.ToCharArray());
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = true)]
        [TestCase("|-",      ExpectedResult = true)]
        [TestCase("|-1",     ExpectedResult = true)]
        [TestCase("abc",     ExpectedResult = false)]
        [TestCase(";abc",    ExpectedResult = false)]
        [TestCase("1abc",    ExpectedResult = false)]
        public bool IsTableRow_Tests(string currentLine)
        {
            return this.target.IsTableRow(currentLine.ToCharArray());
        }

        [TestCase("",        false, ExpectedResult = false)]
        [TestCase(" ",       false, ExpectedResult = false)]
        [TestCase("[NAME]",  false, ExpectedResult = false)]
        [TestCase("#[NAME]", false, ExpectedResult = false)]
        [TestCase("|date|",  false, ExpectedResult = true)]
        [TestCase("|-",      false, ExpectedResult = false)]
        [TestCase("|-1",     false, ExpectedResult = false)]
        [TestCase("abc",     false, ExpectedResult = false)]
        [TestCase(";abc",    false, ExpectedResult = false)]
        [TestCase("1abc",    false, ExpectedResult = false)]
        [TestCase("| x",     false, ExpectedResult = true)]
        [TestCase("| x",     true,  ExpectedResult = false)]
        [TestCase("|-",      true,  ExpectedResult = false)]
        public bool IsTableHeaderRow_Tests(string currentLine, bool passedHeader)
        {
            return this.target.IsTableHeaderRow(currentLine.ToCharArray(), passedHeader);
        }

        [TestCase("",        true,  ExpectedResult = false)]
        [TestCase(" ",       true,  ExpectedResult = false)]
        [TestCase("[NAME]",  true,  ExpectedResult = false)]
        [TestCase("#[NAME]", true,  ExpectedResult = false)]
        [TestCase("|date|",  true,  ExpectedResult = true)]
        [TestCase("|-",      true,  ExpectedResult = false)]
        [TestCase("|-1",     true,  ExpectedResult = false)]
        [TestCase("abc",     true,  ExpectedResult = false)]
        [TestCase(";abc",    true,  ExpectedResult = false)]
        [TestCase("1abc",    true,  ExpectedResult = false)]
        [TestCase("| x",     false, ExpectedResult = false)]
        [TestCase("| x",     true,  ExpectedResult = true)]
        [TestCase("|-",      false, ExpectedResult = false)]
        public bool IsTableDataRow_Tests(string currentLine, bool passedHeader)
        {
            return this.target.IsTableDataRow(currentLine.ToCharArray(), passedHeader);
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = true)]
        [TestCase("|-1",     ExpectedResult = true)]
        [TestCase("abc",     ExpectedResult = false)]
        [TestCase(";abc",    ExpectedResult = false)]
        [TestCase("1abc",    ExpectedResult = false)]
        public bool IsTableHeaderSeparatorRow_Tests(string currentLine)
        {
            return this.target.IsTableHeaderSeparatorRow(currentLine.ToCharArray());
        }

        [TestCase("",        ExpectedResult = true)]
        [TestCase(" ",       ExpectedResult = true)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = false)]
        [TestCase("|-1",     ExpectedResult = false)]
        [TestCase("abc",     ExpectedResult = false)]
        [TestCase(";abc",    ExpectedResult = false)]
        [TestCase("1abc",    ExpectedResult = false)]
        public bool IsEmptyLine_Tests(string currentLine)
        {
            return this.target.IsEmptyLine(currentLine.ToCharArray());
        }
    }
}