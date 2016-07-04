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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace embeddeddata.logic
{
    public class ClassWriter
    {
        private readonly CodeGenerationParameters parameters;
        private readonly string targetNamespace;
        private readonly ProcessNamespace processNamespace = new ProcessNamespace();
        private readonly string inputFilePath;

        public ClassWriter(CodeGenerationParameters parameters, string inputFilePath, string targetNamespace)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (inputFilePath == null) throw new ArgumentNullException(nameof(inputFilePath));
            if (targetNamespace == null) throw new ArgumentNullException(nameof(targetNamespace));
            this.parameters = parameters;
            this.inputFilePath = inputFilePath;
            this.targetNamespace = targetNamespace;
        }

        public void Execute(CodeTextWriter writer, string name, CodeGenerationParameters parameters)
        {
            WriteUsings(writer);

            writer.WriteLine("#region Designer generated code").WriteLine();

            var namespaceResult = this.processNamespace.Execute(this.targetNamespace);

            if (namespaceResult.HasTestData)
            {
                this.WriteTestDataCode(writer, namespaceResult, name);
            }
            else
            {
                var safeName = GenerateSafeClassName(this.inputFilePath);
                this.WriteNonTestDataCode(writer, safeName, name);
            }

            writer.WriteLine().WriteLine("#endregion");
        }

        private void WriteTestDataCode(CodeTextWriter writer, ProcessNamespaceResult namespaceResult, string name)
        {
            writer.WriteLine($"namespace {namespaceResult.Namespace}");
            using (writer.WriteBlock())
            {
                writer.WriteLine("public static partial class Files");
                using (writer.WriteBlock())
                {
                    var innerClassNames = new List<string>();
                    var parts = namespaceResult.SubNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    innerClassNames.AddRange(parts);
                    innerClassNames.Add(Path.GetFileNameWithoutExtension(this.inputFilePath));

                    var blocks = new List<IDisposable>();

                    foreach (var innerClassName in innerClassNames)
                    {
                        writer.WriteLine("public static partial class {0}", GenerateSafeClassName(innerClassName));

                        blocks.Add(writer.WriteBlock());
                    }

                    this.WriteCode(writer, name);

                    foreach (var disposable in blocks)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        private void WriteUsings(CodeTextWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.IO;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Reflection;");
            if (this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("using JetBrains.Annotations;");
            }
            
            writer.WriteLine();
        }

        internal static string GenerateSafeClassName(string inputFilePath)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(inputFilePath);

            if (string.IsNullOrWhiteSpace(nameWithoutExtension))
                throw new InvalidOperationException($@"The Path ""{inputFilePath}"" is invalid.");

            if (nameWithoutExtension.Contains('+'))
            {
                nameWithoutExtension = nameWithoutExtension.Replace("+", "Plus").Replace("-", "Minus");
            }

            if (char.IsNumber(nameWithoutExtension[0]))
            {
                nameWithoutExtension = "_" + nameWithoutExtension;
            }

            return RemoveInvalidCharacters.FromClassName(nameWithoutExtension);
        }

        private void WriteCode(CodeTextWriter writer, string name)
        {
            var resourcePath = this.targetNamespace + "." + name;

            writer.WriteLine($"public const string FileName =\"{name}\";");
            writer.WriteLine($"public const string ResourceName = \"{resourcePath}\";").WriteLine();

            WriteOpenMethod(writer, resourcePath);
            WriteAsFile(writer);
            WriteAsBytes(writer);
            WriteAsString(writer);
            WriteCopyTo(writer);
        }

        private void WriteCopyTo(CodeTextWriter writer)
        {
            if(this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("[NotNull]");
                writer.WriteLine("[Pure]");
            }
            writer.WriteLine("public static string CopyTo([NotNull] string destinationDirectory)");
            using (writer.WriteBlock())
            {
                writer.WriteLine("if (destinationDirectory == null) throw new ArgumentNullException(nameof(destinationDirectory));");
                writer.WriteLine("if (!Directory.Exists(destinationDirectory)) throw new DirectoryNotFoundException(\"destinationDirectory for Resource Data not found\");");
                writer.WriteLine("var fullPath = Path.Combine(destinationDirectory, FileName);");

                writer.WriteLine("using (var fileStream = File.OpenWrite(fullPath))");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("using (var resourceStream = Open())");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("resourceStream.CopyTo(fileStream);");
                    }
                }

                writer.WriteLine("return fullPath;");
            }

        }

        private void WriteAsString(CodeTextWriter writer)
        {
            if (this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("[NotNull]");
                writer.WriteLine("[Pure]");
            }
            writer.WriteLine("public static string AsString()");
            using (writer.WriteBlock())
            {
                writer.WriteLine("return Encoding.UTF8.GetString(AsBytes());");
            }
        }

        private  void WriteAsBytes(CodeTextWriter writer)
        {
            if (this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("[NotNull]");
                writer.WriteLine("[Pure]");
            }
            writer.WriteLine("public static byte[] AsBytes()");
            using (writer.WriteBlock())
            {
                writer.WriteLine("using (var ms = new MemoryStream())");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("using (var ressourceStream = Open())");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("ressourceStream.CopyTo(ms);");

                        writer.WriteLine("return ms.ToArray();");
                    }
                }
            }
        }

        private void WriteAsFile(CodeTextWriter writer)
        {
            if (this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("[NotNull]");
                writer.WriteLine("[Pure]");
            }
            writer.WriteLine("public static void AsFile([NotNull][InstantHandle] Action<string> workWithFile)");
            using (writer.WriteBlock())
            {
                writer.WriteLine("if (workWithFile == null) throw new ArgumentNullException(nameof(workWithFile));");
                writer.WriteLine("var tempFileName = Path.GetTempFileName();");
                writer.WriteLine("try");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("using (var fs = File.OpenWrite(tempFileName))");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("using (var inStream = Open())");
                        using (writer.WriteBlock())
                        {
                            writer.WriteLine("inStream.CopyTo(fs);");
                        }
                    }

                    writer.WriteLine("workWithFile(tempFileName);");
                }
                writer.WriteLine("finally");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("File.Delete(tempFileName);");
                }
            }
        }

        private void WriteOpenMethod(CodeTextWriter writer, string resourcePath)
        {
            if (this.parameters.UseResharperAnnotations)
            {
                writer.WriteLine("[NotNull]");
                writer.WriteLine("[Pure]");
            }
            writer.WriteLine("public static Stream Open()");
            using (writer.WriteBlock())
            {
                string code = $"return Assembly.GetExecutingAssembly().GetManifestResourceStream(\"{resourcePath}\");";

                writer.WriteLine(code);
            }
        }

        private void WriteNonTestDataCode(CodeTextWriter writer, string safeName, string name)
        {
            writer.WriteLine($"namespace {this.targetNamespace}");
            using (writer.WriteBlock())
            {
                writer.WriteLine($"public static class TestData{safeName}");
                using (writer.WriteBlock())
                {
                    this.WriteCode(writer, name);
                }
            }
        }
    }
}