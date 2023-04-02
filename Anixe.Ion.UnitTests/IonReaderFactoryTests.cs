using System;
using NUnit.Framework;
using System.IO;
using System.Text;
using static NExpect.Expectations;
using NExpect;

namespace Anixe.Ion.UnitTests
{
    internal class IonReaderFactoryTests
    {
        [TestCase(null,                             "File path must be defined!",                                TestName = "Throws exception when file path is null")]
        [TestCase("",                               "File path must be defined!",                                TestName = "Throws exception when file path is empty text")]
        [TestCase(" ",                              "File path must be defined!",                                TestName = "Throws exception when file path is whitespace")]
        [TestCase("Examples/not_existing_file.ion", "File '\"Examples/not_existing_file.ion\"' does not exist!", TestName = "Throws exception when file does not exist")]
        public void Throws_Exception_When_FilePath_Does_Not_Exists(string filePath, string expectedExceptionMessage)
        {
            void subject() => IonReaderFactory.Create(filePath);

            Assert.Throws<ArgumentException>(subject, expectedExceptionMessage);
        }

        [Test]
        public void Creates_IonReader_For_Existing_File_Path()
        {
            var actual = IonReaderFactory.Create(FileLoader.GetExamplesIonPath());

            Expect(actual).Not.To.Be.Null();
            Expect(actual).To.Be.An.Instance.Of<IonReader>();
        }

        [Test]
        public void Creates_IonReader_For_FileStream()
        {
            using FileStream fileStream = new FileStream(FileLoader.GetExamplesIonPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ionReader = IonReaderFactory.Create(fileStream);
            Expect(ionReader).Not.To.Be.Null();
            Expect(ionReader).To.Be.An.Instance.Of<IonReader>();
        }

        [Test]
        public void Creates_IonReader_For_MemoryStream()
        {
            var rawFileContent = Encoding.UTF8.GetBytes(File.ReadAllText(FileLoader.GetExamplesIonPath()));

            using MemoryStream fileStream = new MemoryStream(rawFileContent);
            using var ionReader = IonReaderFactory.Create(fileStream);
            Expect(ionReader).Not.To.Be.Null();
            Expect(ionReader).To.Be.An.Instance.Of<IonReader>();
        }
    }
}