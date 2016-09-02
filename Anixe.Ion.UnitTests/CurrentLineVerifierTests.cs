using System;
using System.Linq;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Anixe.Ion;

namespace Anixe.Ion.UnitTests
{
    internal class CurrentLineVerifierTests : AssertionHelper
    {
        private CurrentLineVerifier target;

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
            return this.target.IsSectionHeader(currentLine);
        }

        [TestCase("",        ExpectedResult = false)]
        [TestCase(" ",       ExpectedResult = false)]
        [TestCase("[NAME]",  ExpectedResult = false)]
        [TestCase("#[NAME]", ExpectedResult = false)]
        [TestCase("|date|",  ExpectedResult = false)]
        [TestCase("|-",      ExpectedResult = false)]
        [TestCase("|-1",     ExpectedResult = false)]
        [TestCase("abc",     ExpectedResult = true)]
        [TestCase(";abc",    ExpectedResult = true)]
        [TestCase("1abc",    ExpectedResult = true)]
        public bool IsProperty_Tests(string currentLine)
        {
            return this.target.IsProperty(currentLine);
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
            return this.target.IsComment(currentLine);
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
            return this.target.IsTableRow(currentLine);
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
            return this.target.IsTableHeaderSeparatorRow(currentLine);
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
            return this.target.IsEmptyLine(currentLine);
        }
    }
}