using NUnit.Framework;
using System;

namespace Anixe.Ion.UnitTests
{
    internal class SectionHeaderReaderTests
    {
        private SectionHeaderReader target = null!;

        [SetUp]
        public void Before_Each_Test()
        {
            this.target = new SectionHeaderReader();
        }

        [TestCase("[ABC]",       ExpectedResult = "ABC")]
        [TestCase("[ABC]xyz",    ExpectedResult = "ABC")]
        [TestCase("[ABC][1]xyz", ExpectedResult = "ABC")]
        [TestCase("[ABC] ",      ExpectedResult = "ABC")]
        public string Read_Tests(string currentLine)
        {
            var actual = this.target.Read(currentLine.ToCharArray());
            return new string(actual.Array, actual.Offset, actual.Count);
        }
    }
}