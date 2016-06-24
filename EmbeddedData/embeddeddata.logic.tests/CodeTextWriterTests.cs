using System;

using NUnit.Framework;

namespace embeddeddata.logic.tests
{
    [TestFixture]
    public sealed class CodeTextWriterTests
    {
        [Test]
        public void WriteLineWithoutParameterAppendsNewLine()
        {
            var sut = new CodeTextWriter();

            sut.WriteLine();

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo(Environment.NewLine));
        }

        [Test]
        public void WriteLineWithText()
        {
            var sut = new CodeTextWriter();

            sut.WriteLine("abcd");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("abcd" + Environment.NewLine));
        }

        [Test]
        public void WriteLineWithFormat()
        {
            var sut = new CodeTextWriter();

            sut.WriteLine("abcd{0}", 23);

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("abcd23" + Environment.NewLine));
        }

        [Test]
        public void WriteLineRespectsIndent()
        {
            var sut = new CodeTextWriter();
            sut.IncreaseIndent();
            sut.WriteLine("abcd");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("  abcd" + Environment.NewLine));
        }

        [Test]
        public void WriteBlock()
        {
            var sut = new CodeTextWriter();

            sut.WriteLine("public class Shibby");
            using (sut.WriteBlock())
            {
                sut.WriteLine("public void Gulu(){}");
            }

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo(@"public class Shibby
{
  public void Gulu(){}
}
"));
        }
    }
}
