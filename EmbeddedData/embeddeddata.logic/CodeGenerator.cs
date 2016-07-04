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
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace embeddeddata.logic
{
    public class CodeGenerator
    {
        private readonly string inputFilePath;
        private readonly string targetNamespace;

        public CodeGenerator(string inputFilePath, string targetNamespace)
        {
            if (targetNamespace == null) throw new ArgumentNullException(nameof(targetNamespace));
            if (string.IsNullOrWhiteSpace(inputFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFilePath));

            this.inputFilePath = inputFilePath;
            this.targetNamespace = targetNamespace;
        }

        public string Generate()
        {
            var writer = new CodeTextWriter("    ");

            var parameters = new CodeGenerationParameters.Builder
            {
                UseResharperAnnotations = true,
            }.Build();

            string name = Path.GetFileName(this.inputFilePath);

            var classWriter = new ClassWriter(parameters, this.inputFilePath, this.targetNamespace);

            classWriter.Execute(writer, name);

            return writer.ToString();
        }
    }
}
