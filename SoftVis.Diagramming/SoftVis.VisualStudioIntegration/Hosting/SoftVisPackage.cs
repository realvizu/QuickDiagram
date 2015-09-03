using Codartis.SoftVis.VisualStudioIntegration.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(DiagramToolWindow))]
    public sealed class SoftVisPackage : Package, IWindowManager
    {
        private const string PackageGuidString = "198d9322-583a-4112-a2a8-61f4c0818966";

        private readonly List<CommandBase> _commands = new List<CommandBase>();

        private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider _globalServiceProvider;

        private const double MinFontSize = 6;
        private const double MaxFontSize = 24;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var sourceDocumentProvider = new SourceDocumentProvider(this);
            DiagramBuilder.Initialize(sourceDocumentProvider);

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            _commands.Add(new ShowDiagramWindowCommand(this, this));
            _commands.Add(new ClearDiagramCommand(this, this));
            _commands.Add(new AddToDiagramCommand(this, this));
            _commands.Add(new IncreaseFontSizeCommand(this, this));
            _commands.Add(new DecreaseFontSizeCommand(this, this));
            _commands.Add(new CopyToClipboardCommand(this, this));
            _commands.Add(new ExportToFileCommand(this, this));
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

        public void ShowDiagramWindow()
        {
            var toolWindow = GetDiagramWindow();
            var windowFrame = (IVsWindowFrame)toolWindow.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public void IncreaseFontSize()
        {
            var window = GetDiagramWindow();
            window.DiagramFontSize = Math.Min(window.DiagramFontSize + 1, MaxFontSize);
        }

        public void DecreaseFontSize()
        {
            var window = GetDiagramWindow();
            window.DiagramFontSize = Math.Max(window.DiagramFontSize - 1, MinFontSize);
        }

        public DiagramToolWindow GetDiagramWindow()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var toolWindow = (DiagramToolWindow)FindToolWindow(typeof(DiagramToolWindow), 0, true);
            if ((null == toolWindow) || (null == toolWindow.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            return toolWindow;
        }
    }
}
