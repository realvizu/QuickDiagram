using Codartis.SoftVis.VisualStudioIntegration.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Presentation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using IVisualStudioServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

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
    public sealed class SoftVisPackage : Package, IHostServices, IHostServiceProvider
    {
        private const string PackageGuidString = "198d9322-583a-4112-a2a8-61f4c0818966";
        private const int SingletonToolWindowInstanceId = 0;

        private readonly List<CommandBase> _commands = new List<CommandBase>();
        private IVisualStudioServiceProvider _globalServiceProvider;
        private ISourceDocumentProvider _sourceDocumentProvider;
        private DiagramToolWindow _diagramToolWindow;

        protected override void Initialize()
        {
            base.Initialize();

            _globalServiceProvider = GetGlobalService(typeof(IVisualStudioServiceProvider)) as IVisualStudioServiceProvider;
            _sourceDocumentProvider = new SourceDocumentProvider(this);
            _diagramToolWindow = CreateToolWindow(_sourceDocumentProvider);

            InitializeCommands();
        }

        private DiagramToolWindow CreateToolWindow(ISourceDocumentProvider sourceDocumentProvider)
        {
            var toolWindow = CreateToolWindow(typeof(DiagramToolWindow), SingletonToolWindowInstanceId) as DiagramToolWindow;
            if (toolWindow == null || toolWindow.Frame == null)
                throw new NotSupportedException("Cannot create tool window.");

            toolWindow.Initialize(sourceDocumentProvider);

            return toolWindow;
        }

        private void InitializeCommands()
        {
            InitializeCommand(i => new AddToDiagramCommand(i));
            InitializeCommand(i => new ShowDiagramWindowCommand(i));
            InitializeCommand(i => new ClearDiagramCommand(i));
            InitializeCommand(i => new IncreaseFontSizeCommand(i));
            InitializeCommand(i => new DecreaseFontSizeCommand(i));
            InitializeCommand(i => new CopyToClipboardCommand(i));
            InitializeCommand(i => new ExportToFileCommand(i));
        }

        private void InitializeCommand<TCommand>(Func<IHostServices, TCommand> constructorDelegate)
            where TCommand : CommandBase
        {
            var command = constructorDelegate(this);
            _commands.Add(command);
            AddMenuCommand(this, command);
        }

        private void AddMenuCommand(IServiceProvider serviceProvider, CommandBase command)
        {
            var commandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(command.CommandSet, command.CommandId);
                var menuItem = new MenuCommand(command.Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public IDiagramWindow GetDiagramWindow()
        {
            return _diagramToolWindow;
        }

        public TServiceInterface GetService<TServiceInterface, TService>() 
            where TServiceInterface : class 
            where TService : class
        {
            return (TServiceInterface)GetService(_globalServiceProvider, typeof(TService).GUID, false);
        }

        private static object GetService(IVisualStudioServiceProvider serviceProvider, Guid guidService, bool unique)
        {
            var riid = VSConstants.IID_IUnknown;
            var ppvObject = IntPtr.Zero;
            object obj = null;
            if (serviceProvider.QueryService(ref guidService, ref riid, out ppvObject) == 0)
            {
                if (ppvObject != IntPtr.Zero)
                {
                    try
                    {
                        obj = !unique
                            ? Marshal.GetObjectForIUnknown(ppvObject)
                            : Marshal.GetUniqueObjectForIUnknown(ppvObject);
                    }
                    finally
                    {
                        Marshal.Release(ppvObject);
                    }
                }
            }
            return obj;
        }

    }
}
