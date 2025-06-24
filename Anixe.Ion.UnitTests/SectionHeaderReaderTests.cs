using Xunit;

namespace Anixe.Ion.UnitTests
{
    public class SectionHeaderReaderTests
    {
        private readonly SectionHeaderReader target;

        public SectionHeaderReaderTests()
        {
            this.target = new SectionHeaderReader();
        }

        [Theory]
        [InlineData("[ABC]",       "ABC")]
        [InlineData("[ABC]xyz",    "ABC")]
        [InlineData("[ABC][1]xyz", "ABC")]
        [InlineData("[ABC] ",      "ABC")]
        public void Read_Tests(string currentLine, string expected)
        {
            var actual = this.target.Read(new System.ArraySegment<char>(currentLine.ToCharArray()));
            var result = new string(actual.Array!, actual.Offset, actual.Count);
            Assert.Equal(expected, result);
        }
    }
}