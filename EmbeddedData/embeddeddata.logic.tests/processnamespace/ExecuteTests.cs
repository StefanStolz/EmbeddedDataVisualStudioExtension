﻿#region File Header

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
        public void WithSubNamespace()
        {
            const string Namespace = "notenbox.nbw7fileadapter.tests.TestData.SubFolder";

            var sut = new ProcessNamespace();

            var result = sut.Execute(Namespace);

            Assert.That(result.Namespace, Is.EqualTo("notenbox.nbw7fileadapter.tests.TestData"));
            Assert.That(result.HasTestData, Is.True);
            Assert.That(result.SubNamespace, Is.EqualTo("SubFolder"));
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
    }
}
