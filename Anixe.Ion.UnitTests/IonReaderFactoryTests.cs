using System;
using System.IO;
using System.Text;
using Xunit;

namespace Anixe.Ion.UnitTests
{
    public class IonReaderFactoryTests
    {
        [Theory]
        [InlineData(null,                             "File path must be defined!")] // Throws exception when file path is null
        [InlineData("",                               "File path must be defined!")] // Throws exception when file path is empty text
        [InlineData(" ",                              "File path must be defined!")] // Throws exception when file path is whitespace
        [InlineData("Examples/not_existing_file.ion", "File 'Examples/not_existing_file.ion' does not exist!")] // Throws exception when file does not exist
        public void Throws_Exception_When_FilePath_Does_Not_Exists(string? filePath, string expectedExceptionMessage)
        {
#pragma warning disable CS8604 // Possible null reference argument. - intended!
            void subject() => IonReaderFactory.Create(filePath);
#pragma warning restore CS8604

            var ex = Assert.Throws<ArgumentException>(subject);
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }

        [Fact]
        public void Creates_IonReader_For_Existing_File_Path()
        {
            var actual = IonReaderFactory.Create(FileLoader.GetExamplesIonPath());

            Assert.NotNull(actual);
            Assert.IsType<IonReader>(actual);
        }

        [Fact]
        public void Creates_IonReader_For_FileStream()
        {
            using FileStream fileStream = new FileStream(FileLoader.GetExamplesIonPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ionReader = IonReaderFactory.Create(fileStream);
            Assert.NotNull(ionReader);
            Assert.IsType<IonReader>(ionReader);
        }

        [Fact]
        public void Creates_IonReader_For_MemoryStream()
        {
            var rawFileContent = Encoding.UTF8.GetBytes(File.ReadAllText(FileLoader.GetExamplesIonPath()));

            using MemoryStream fileStream = new MemoryStream(rawFileContent);
            using var ionReader = IonReaderFactory.Create(fileStream);
            Assert.NotNull(ionReader);
            Assert.IsType<IonReader>(ionReader);
        }
    }
}