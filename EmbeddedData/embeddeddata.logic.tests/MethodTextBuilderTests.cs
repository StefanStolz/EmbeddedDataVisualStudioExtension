using System.IO;
using System.Text;

using NUnit.Framework;

namespace embeddeddata.logic.tests
{
    [TestFixture]
    public sealed class MethodTextBuilderTests
    {
        [Test]
        public void WriteSimpleWithMethodWithoutParameters()
        {
            var sut = MethodTextBuilder.Create("Simple");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("void Simple()"));
        }

        [Test]
        public void WriteVoidMethodWithSingleStringParamter()
        {
            var sut = MethodTextBuilder.Create("Simple");
            sut.AddParameter("string", "parameter");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("void Simple(string parameter)"));
        }

        [Test]
        public void WriteVoidMethodWithTwoParameters()
        {
            var result = MethodTextBuilder.Create("Simple")
                .AddParameter("string", "parameter")
                .AddParameter("int", "value")
                .ToString();

            Assert.That(result, Is.EqualTo("void Simple(string parameter, int value)"));
        }

        [Test]
        public void WriteMethodWithParameterWithAttributes()
        {
            var sut = MethodTextBuilder.Create("Shibby");
            var parameterDefinition = sut.BeginParameter();
            parameterDefinition.AddAttribute("NotNull");
            parameterDefinition.SetType("string");
            parameterDefinition.SetName("parameter");
            parameterDefinition.Finish();

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("void Shibby([NotNull] string parameter)"));
        }

        [Test]
        public void UseFluentApiOfMethdTextBuilder()
        {
            var sut = MethodTextBuilder.Create("Gulu")
                .BeginParameter()
                .SetName("parameter")
                .SetType("string")
                .AddAttribute("NotNull")
                .Finish()
                .BeginParameter()
                .SetType("int")
                .SetName("value")
                .Finish()
                .SetReturnType("int");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("int Gulu([NotNull] string parameter, int value)"));
        }

        [Test]
        public void WriteMethodThatReturnsAnInteger()
        {
            var sut = MethodTextBuilder.Create("Simple")
                .SetReturnType("int");

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("int Simple()"));
        }

        [Test]
        public void WriteToCodeWriter()
        {
            var cw = new CodeTextWriter("  ");

            using (cw.WriteBlock())
            {
                MethodTextBuilder.Create("Simple").WriteTo(cw);
            }

            var result = cw.ToString();

            Assert.That(result, Is.EqualTo(@"{
  void Simple()
}
"));
        }
    }
}
