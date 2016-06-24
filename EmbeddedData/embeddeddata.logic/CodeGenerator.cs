using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private static readonly List<char> validClassNameCharacters = new List<char>
            (
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_"
            );


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

        public void xxx(string workingDirectory)
        {
            if (workingDirectory == null) throw new ArgumentNullException(nameof(workingDirectory));
            if (!Directory.Exists(workingDirectory)) throw new DirectoryNotFoundException("OutputDirectory for Resource Data not found");
        }

        public string Generate()
        {
            var writer = new CodeTextWriter();

            string name = Path.GetFileName(this.inputFilePath);

            var safeName = GenerateSafeClassName(this.inputFilePath);

            writer.WriteLine("using System;");
            writer.WriteLine("using System.IO;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Reflection;");
            writer.WriteLine("using JetBrains.Annotations;");
            writer.WriteLine();

            writer.WriteLine("#region Designer generated code").WriteLine();

            var processNamespace = new ProcessNamespace();

            var namespaceResult = processNamespace.Execute(this.targetNamespace);

            if (namespaceResult.HasTestData)
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
            else
            {
                this.WriteNonTestDataCode(writer, safeName, name);
            }

            writer.WriteLine().WriteLine("#endregion");

            return writer.ToString();
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

        internal static string GenerateSafeClassName(string inputFilePath)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(inputFilePath);

            if (string.IsNullOrWhiteSpace(nameWithoutExtension)) throw new InvalidOperationException($@"Der Pfad ""{inputFilePath}"" ist ungültig.");

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
    }
}