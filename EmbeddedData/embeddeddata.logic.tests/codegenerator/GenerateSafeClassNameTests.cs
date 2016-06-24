using System;

using NUnit.Framework;

namespace embeddeddata.logic.tests.codegenerator
{
    [TestFixture]
    public sealed class GenerateSafeClassNameTests
    {
        [Test]
        public void GenerateForRealNbw()
        {
            var result = CodeGenerator.GenerateSafeClassName(@"N:\Shibby\Real.nbw");

            Assert.That(result, Is.EqualTo("Real"));
        }
    }
}
