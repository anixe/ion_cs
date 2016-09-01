using System;
using NUnit.Framework;
using Anixe.Ion;
using System.IO;
using System.Text;

namespace Anixe.Ion.UnitTests
{
    internal class IonReaderFactoryTests : AssertionHelper
    {
        [TestCase(null,                             "File path must be defined!",                                TestName = "Throws exception when file path is null")]
        [TestCase("",                               "File path must be defined!",                                TestName = "Throws exception when file path is empty text")]
        [TestCase(" ",                              "File path must be defined!",                                TestName = "Throws exception when file path is whitespace")]
        [TestCase("Examples/not_existing_file.ion", "File '\"Examples/not_existing_file.ion\"' does not exist!", TestName = "Throws exception when file does not exist")]
        public void Throws_Exception_When_FilePath_Does_Not_Exists(string filePath, string expectedExceptionMessage)
        {
            TestDelegate subject = () => IonReaderFactory.Create(filePath);

            Assert.Throws<ArgumentException>(subject, expectedExceptionMessage);
        }

        [Test]
        public void Creates_IonReader_For_Existing_File_Path()
        {
            var actual = IonReaderFactory.Create("Examples/example.ion");

            Expect(actual, Is.Not.Null);
            Expect(actual, Is.TypeOf<IonReader>());
        }

        [Test]
        public void Creates_IonReader_For_FileStream()
        {
            using(FileStream fileStream = new FileStream("Examples/example.ion", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using(var ionReader = IonReaderFactory.Create(fileStream))
                {
                    Expect(ionReader, Is.Not.Null);
                    Expect(ionReader, Is.TypeOf<IonReader>());
                }
            }
        }

        [Test]
        public void Creates_IonReader_For_MemoryStream()
        {
            var rawFileContent = Encoding.UTF8.GetBytes(File.ReadAllText("Examples/example.ion"));

            using(MemoryStream fileStream = new MemoryStream(rawFileContent))
            {
                using(var ionReader = IonReaderFactory.Create(fileStream))
                {
                    Expect(ionReader, Is.Not.Null);
                    Expect(ionReader, Is.TypeOf<IonReader>());
                }
            }
        }
    }
}