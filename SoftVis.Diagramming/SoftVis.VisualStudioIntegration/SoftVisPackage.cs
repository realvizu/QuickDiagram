using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(SoftVisPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(DiagramToolWindow))]
    public sealed class SoftVisPackage : Package
    {
        public const string PackageGuidString = "198d9322-583a-4112-a2a8-61f4c0818966";
        public const string CommandSetGuidString = "3ec3e947-3047-4579-a09e-921b99ce5789";
        public const uint DiagramToolWindowCommandId = 0x101;

        private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider _globalServiceProvider;

        public RoslynBasedUmlModel Model { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftVisPackage"/> class.
        /// </summary>
        public SoftVisPackage()
        {
            Model = new RoslynBasedUmlModel();
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            DiagramToolWindowCommand.Initialize(this);
            SourceDocumentProvider.Initialize(this);
        }

        public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GlobalServiceProvider
        {
            get
            {
                if (_globalServiceProvider == null)
                {
                    var serviceProviderType = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
                    _globalServiceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)GetGlobalService(serviceProviderType);
                }
                return _globalServiceProvider;
            }
        }
    }
}
