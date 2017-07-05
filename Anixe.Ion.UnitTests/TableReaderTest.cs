using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Anixe.Ion.UnitTests
{
    public class TableReaderTest : AssertionHelper
    {
        class TestModel
        {
          public string id;
          public string[] descriptions;
        }

        private IIonReader target;
        private List<Action> assertions;

        [TestFixtureSetUp]
        public void Before_All_Test()
        {
            var rootPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
            this.target = IonReaderFactory.Create(Path.Combine(rootPath, "Examples", "example.ion"));
        }

        [TestFixtureTearDown]
        public void After_All_Test()
        {
            this.target.Dispose();
        }

        [Test]
        public void Fires_Handler_For_Each_Row()
        {
            var reader = new SectionReader(this.target);
            List<TestModel> actual = null;
            reader.RegisterHandler("DEF.SECTION", (sectionName) => {
              var tableReader = new TableReader<TestModel>(this.target, "DEF.SECTION", (columns) => {
                var tm = new TestModel{
                  id = columns[0],
                  descriptions = new string[] { columns[0], columns[1] }
                };
                return tm;
              });

              actual = tableReader.Read();
            });
            reader.Read();

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("123", actual[0].id);
        }
    }
}