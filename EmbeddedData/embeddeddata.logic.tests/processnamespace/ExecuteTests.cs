using System;

using NUnit.Framework;

namespace embeddeddata.logic.tests.processnamespace
{
    [TestFixture]
    public sealed class ExecuteTests
    {
        [Test]
        public void NoSubNamespaceIfNoTestDataInNamespace()
        {
            const string Namespace = "notenbox.nbw7fileadapter.tests.SubFolder";

            var sut = new ProcessNamespace();

            var result = sut.Execute(Namespace);

            Assert.That(result.Namespace, Is.EqualTo(Namespace));
            Assert.That(result.HasTestData, Is.False);
            Assert.That(result.SubNamespace, Is.Empty);
        }

        [Test]
        public void WithTestDataAtTheEnd()
        {
            const string Namespace = "notenbox.nbw7fileadapter.tests.TestData";

            var sut = new ProcessNamespace();

            var result = sut.Execute(Namespace);

            Assert.That(result.Namespace, Is.EqualTo("notenbox.nbw7fileadapter.tests.TestData"));
            Assert.That(result.HasTestData, Is.True);
            Assert.That(result.SubNamespace, Is.Empty);
        }

        [Test]
        public void WithSubNamespace()
        {
            const string Namespace = "notenbox.nbw7fileadapter.tests.TestData.SubFolder";

            var sut = new ProcessNamespace();

            var result = sut.Execute(Namespace);

            Assert.That(result.Namespace, Is.EqualTo("notenbox.nbw7fileadapter.tests.TestData"));
            Assert.That(result.HasTestData, Is.True);
            Assert.That(result.SubNamespace, Is.EqualTo("SubFolder"));
        }
    }
}
