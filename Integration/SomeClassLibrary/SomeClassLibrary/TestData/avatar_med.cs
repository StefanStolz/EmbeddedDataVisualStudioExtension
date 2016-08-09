using System;
using System.IO;
using System.Text;
using System.Reflection;

#region Designer generated code

// Effective Configuration
// UseResharperAnnotations: False
// ResharperAnnotationNamespace: JetBrains.Annotations
// ClassNameWithExtension: False
namespace SomeClassLibrary.TestData
{
    public static partial class Files
    {
        public static partial class avatar_med
        {
            public const string FileName ="avatar_med.jpg";
            public const string ResourceName = "SomeClassLibrary.TestData.avatar_med.jpg";
            
            public static Stream Open()
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream("SomeClassLibrary.TestData.avatar_med.jpg");
            }
            public static void AsFile(Action<string> workWithFile)
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
            }
            public static byte[] AsBytes()
            {
                using (var ms = new MemoryStream())
                {
                    using (var ressourceStream = Open())
                    {
                        ressourceStream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            public static string AsString()
            {
                return Encoding.UTF8.GetString(AsBytes());
            }
            public static string CopyTo(string destinationDirectory)
            {
                if (destinationDirectory == null) throw new ArgumentNullException(nameof(destinationDirectory));
                if (!Directory.Exists(destinationDirectory)) throw new DirectoryNotFoundException("destinationDirectory for Resource Data not found");
                var fullPath = Path.Combine(destinationDirectory, FileName);
                using (var fileStream = File.OpenWrite(fullPath))
                {
                    using (var resourceStream = Open())
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
                return fullPath;
            }
            public static FileHandle OpenFile()
            {
                var tempFileName = Path.GetTempFileName();
                using (var destination = File.OpenWrite(tempFileName))
                {
                    using (var source = Open())
                    {
                        source.CopyTo(destination);
                    }
                }
                return new FileHandle(tempFileName);
            }
            public class FileHandle : IDisposable
            {
                private bool disposed;
                public FileHandle(string path)
                {
                    this.Path = path;
                }
                public string Path { get; }
                public void Delete()
                {
                    File.Delete(this.Path);
                }
                private void Dispose(bool disposing)
                {
                    if(!this.disposed)
                    {
                        if(disposing)
                        {
                            File.Delete(this.Path);
                        }
                        this.disposed = true;
                    }
                }
                public void Dispose()
                {
                    GC.SuppressFinalize(this);
                    this.Dispose(true);
                }
            }
        }
    }
}

#endregion
