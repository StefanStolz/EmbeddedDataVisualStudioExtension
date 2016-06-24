#region File Header

// The MIT License (MIT)
// 
// Copyright (c) 2016 Stefan Stolz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region using directives

using System;

using NUnit.Framework;

#endregion

namespace embeddeddata.logic.tests
{
    [TestFixture]
    public sealed class CodeTextWriterTests
    {
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
        public void WriteLineWithFormat()
        {
            var sut = new CodeTextWriter();

            sut.WriteLine("abcd{0}", 23);

            var result = sut.ToString();

            Assert.That(result, Is.EqualTo("abcd23" + Environment.NewLine));
        }

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
    }
}
