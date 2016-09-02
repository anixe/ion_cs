using NUnit.Framework;
using Anixe.Ion;

namespace Anixe.Ion.UnitTests
{
    internal class SectionHeaderReaderTests : AssertionHelper
    {
        private SectionHeaderReader target;

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
            return this.target.Read(currentLine);
        }
    }
}