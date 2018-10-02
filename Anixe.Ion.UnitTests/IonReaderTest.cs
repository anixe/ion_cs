using System;
using System.IO;
using NUnit.Framework;

namespace Anixe.Ion.UnitTests
{
    [TestFixture]
    public class IonReaderTest
    {
        [Test]
        public void Should_Read_Ins_Ion()
        {
            int counter = 0;
            using (var stream = File.OpenRead(FileLoader.GetInsIonPath()))
            using (var reader = IonReaderFactory.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.CurrentSection != "INSURANCE")
                    {
                        continue;
                    }
                    if (reader.IsEmptyLine)
                    {
                        break;
                    }
                    if (reader.IsTableDataRow)
                    {
                        counter++;
                        var columns = reader.CurrentLine.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        Assert.AreEqual(12, columns.Length);
                    }
                }
            }
            Assert.AreEqual(3, counter);
        }
    }
}