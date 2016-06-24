using System;

using NUnit.Framework;

namespace embeddeddata.logic.tests.processnamespace
{
    [TestFixture]
    public sealed class GetBaseNamespaceTests
    {
        [Test]
        public void ItemInTestDataRoot()
        {
            var result = ProcessNamespace.GetBaseNamespace("notenbox.nbw7fileadapter.tests.TestData");

            Assert.That(result, Is.EqualTo("notenbox.nbw7fileadapter.tests.TestData"));
        }

        [Test]
        public void ItemInSubfolder()
        {
            var result = ProcessNamespace.GetBaseNamespace("notenbox.nbw7fileadapter.tests.TestData.SubFolder");

            Assert.That(result, Is.EqualTo("notenbox.nbw7fileadapter.tests.TestData"));
        }

        [Test]
        public void ExceptionIfNoTestDataInNamespace()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    ProcessNamespace.GetBaseNamespace("notenbox.nbw7fileadapter.tests.SubFolder");
                });
        }
    }
}
