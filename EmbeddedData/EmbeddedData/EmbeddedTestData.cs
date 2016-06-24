using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using VSLangProj80;

using EmbeddedTestDataCodeGenerator = embeddeddata.logic.CodeGenerator;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace EmbeddedData
{
    [ComVisible(true)]
    [Guid("3742A3C2-2C1D-435E-9A96-8BCF2D9FD5A5")]
    [ProvideObject(typeof(EmbeddedTestData))]
    [CodeGeneratorRegistration(typeof(EmbeddedTestData), "EmbeddedData", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(EmbeddedTestData), "EmbeddedData", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    public class EmbeddedTestData : IVsSingleFileGenerator, IObjectWithSite
    {

        private object site = null;
        private CodeDomProvider codeDomProvider = null;
        private ServiceProvider serviceProvider = null;

        private CodeDomProvider CodeProvider
        {
            get
            {
                if (this.codeDomProvider == null)
                {
                    IVSMDCodeDomProvider provider = (IVSMDCodeDomProvider)this.SiteServiceProvider.GetService(typeof(IVSMDCodeDomProvider).GUID);
                    if (provider != null)
                        this.codeDomProvider = (CodeDomProvider)provider.CodeDomProvider;
                }
                return this.codeDomProvider;
            }
        }

        private ServiceProvider SiteServiceProvider
        {
            get
            {
                if (this.serviceProvider == null)
                {
                    IServiceProvider oleServiceProvider = this.site as IServiceProvider;
                    this.serviceProvider = new ServiceProvider(oleServiceProvider);
                }
                return this.serviceProvider;
            }
        }

        #region IVsSingleFileGenerator

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = "." + this.CodeProvider.FileExtension;
            return VSConstants.S_OK;
        }

 
        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            if (bstrInputFileContents == null)
                throw new ArgumentException(nameof(bstrInputFileContents));

            byte[] bytes;
            try
            {
                var codeGenerator = new EmbeddedTestDataCodeGenerator(wszInputFilePath, wszDefaultNamespace);

                bytes = Encoding.UTF8.GetBytes(codeGenerator.Generate());
            }
            catch (Exception exception)
            {
                bytes = Encoding.UTF8.GetBytes(exception.Message);
            }

            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
            pcbOutput = (uint)bytes.Length;

            return VSConstants.S_OK;
        }

        #endregion IVsSingleFileGenerator

        #region IObjectWithSite

        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (this.site == null)
                Marshal.ThrowExceptionForHR(VSConstants.E_NOINTERFACE);

            // Query for the interface using the site object initially passed to the generator 
            IntPtr punk = Marshal.GetIUnknownForObject(this.site);
            int hr = Marshal.QueryInterface(punk, ref riid, out ppvSite);
            Marshal.Release(punk);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
        }

        public void SetSite(object pUnkSite)
        {
            // Save away the site object for later use 
            this.site = pUnkSite;

            // These are initialized on demand via our private CodeProvider and SiteServiceProvider properties 
            this.codeDomProvider = null;
            this.serviceProvider = null;
        }

        #endregion IObjectWithSite

    }
}