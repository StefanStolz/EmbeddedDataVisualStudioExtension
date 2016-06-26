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
        private const string CodeForAsFile = @"        
    public static void AsFile([NotNull] Action<string> workWithFile)
    {
        if (workWithFile == null) throw new ArgumentNullException(nameof(workWithFile));
        var tempFileName = Path.GetTempFileName();
        try
        {
            using (var fs = File.OpenWrite(tempFileName))
            {
                using (var inStream = Open())
                {
                    inStream.CopyTo(fs);                        
                }
            }

            workWithFile(tempFileName);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }";

        private const string CodeForAsBytes = @"
    public static byte[] AsBytes()
    {
        using (var ms = new MemoryStream())
        {
            using(var ressourceStream = Open())
            {
                ressourceStream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }";

        private const string CodeForAsString = @"
    public static string AsString()
    {
        return Encoding.UTF8.GetString(AsBytes());
    }";

        private const string CodeForCopyTo = @"
    public static string CopyTo(string workingDirectory)
    {
        if (workingDirectory == null) throw new ArgumentNullException(nameof(workingDirectory));
        if (!Directory.Exists(workingDirectory)) throw new DirectoryNotFoundException(""OutputDirectory for Resource Data not found"");
        
        var fullPath = Path.Combine(workingDirectory, FileName);
        
        using (var fileStream = File.OpenWrite(fullPath))
        {
            using (var resourceStream = Open())
            {
                resourceStream.CopyTo(fileStream);
            }
        }

        return fullPath;
    }";

        private static readonly List<char> validClassNameCharacters = new List<char>
            (
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_"
            );
        private readonly string inputFilePath;
        private readonly string targetNamespace;
        private readonly ProcessNamespace processNamespace;

        public CodeGenerator(string inputFilePath, string targetNamespace)
        {
            if (targetNamespace == null) throw new ArgumentNullException(nameof(targetNamespace));
            if (string.IsNullOrWhiteSpace(inputFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFilePath));

            this.inputFilePath = inputFilePath;
            this.targetNamespace = targetNamespace;
            processNamespace = new ProcessNamespace();
        }

        public string Generate()
        {
            var writer = new CodeTextWriter();

            string name = Path.GetFileName(this.inputFilePath);

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

            return writer.ToString();
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

        private static void WriteUsings(CodeTextWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.IO;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Reflection;");
            writer.WriteLine("using JetBrains.Annotations;");
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

            return new string(nameWithoutExtension.Where(c => validClassNameCharacters.Contains(c)).ToArray());
        }

        private void WriteCode(CodeTextWriter writer, string name)
        {
            var resourcePath = this.targetNamespace + "." + name;

            writer.WriteLine($"public const string FileName =\"{name}\";");
            writer.WriteLine($"public const string ResourceName = \"{resourcePath}\";").WriteLine();

            writer.WriteLine("public static Stream Open()");
            using (writer.WriteBlock())
            {
                string code = $"return Assembly.GetExecutingAssembly().GetManifestResourceStream(\"{resourcePath}\");";

                writer.WriteLine(code);
            }

            writer.WriteLine(CodeForAsFile);
            writer.WriteLine(CodeForAsBytes);
            writer.WriteLine(CodeForAsString);
            writer.WriteLine(CodeForCopyTo);
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
