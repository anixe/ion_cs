using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Anixe.Ion.UnitTests
{
    public class SectionReaderTest : AssertionHelper
    {
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
        public void Fires_Event_For_Section()
        {
            var reader = new SectionReader(this.target);
            reader.RegisterHandler("DEF.SECTION", (sectionName) => {
              Assert.AreEqual("DEF.SECTION", sectionName);
            });
            reader.RegisterHandler("DEF.SECTIONXXX", (sectionName) => {
              Assert.Fail();
            });
            reader.Read();
        }        
    }
}