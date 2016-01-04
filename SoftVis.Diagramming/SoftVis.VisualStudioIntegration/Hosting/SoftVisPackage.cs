using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Codartis.SoftVis.VisualStudioIntegration.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Commands.EventTriggered;
using Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Building;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
    public sealed class SoftVisPackage : Package, IPackageServices, IHostServiceProvider
    {
        private const string PackageGuidString = "198d9322-583a-4112-a2a8-61f4c0818966";
        private const int SingletonToolWindowInstanceId = 0;

        private readonly List<ShellTriggeredCommandBase> _shellTriggeredCommands = new List<ShellTriggeredCommandBase>();
        private readonly Dictionary<Type, CommandBase> _eventTriggeredCommands = new Dictionary<Type, CommandBase>();

        private IVisualStudioServiceProvider _visualStudioServiceProvider;
        private IComponentModel _componentModel;

        private IWorkspaceServices _workspaceServices;
        private IModelServices _modelBuilder;
        private DiagramManager _diagramManager;

        protected override void Initialize()
        {
            base.Initialize();

            // This is needed otherwise VS catches the exception and shows no stack trace.
            Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) => { Debugger.Break(); };

            _visualStudioServiceProvider = GetGlobalService(typeof(IVisualStudioServiceProvider)) as IVisualStudioServiceProvider;
            if (_visualStudioServiceProvider == null)
                throw new Exception("Unable to get IVisualStudioServiceProvider.");

            _componentModel = GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (_componentModel == null)
                throw new Exception("Unable to get IComponentModel.");

            _workspaceServices = new WorkspaceServices(this);
            _modelBuilder = new RoslynBasedModelBuilder(_workspaceServices);
            _diagramManager = new DiagramManager(_modelBuilder.Model, CreateToolWindow());
            _diagramManager.PackageEvent += OnPackageEvent;

            InitializeShellTriggeredCommands();
            InitializeEventTriggeredCommands();
        }

        public IWorkspaceServices GetWorkspaceServices()
        {
            return _workspaceServices;
        }

        public IModelServices GetModelServices()
        {
            return _modelBuilder;
        }

        public IDiagramServices GetDiagramServices()
        {
            return _diagramManager;
        }

        public IVsRunningDocumentTable GetRunningDocumentTableService()
        {
            return GetVisualStudioService<IVsRunningDocumentTable, SVsRunningDocumentTable>();
        }

        public VisualStudioWorkspace GetVisualStudioWorkspace()
        {
            return _componentModel.GetService<VisualStudioWorkspace>();
        }

        private TServiceInterface GetVisualStudioService<TServiceInterface, TService>()
            where TServiceInterface : class
            where TService : class
        {
            return (TServiceInterface)GetVisualStudioService(_visualStudioServiceProvider, typeof(TService).GUID, false);
        }

        private static object GetVisualStudioService(IVisualStudioServiceProvider serviceProvider, Guid guidService, bool unique)
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

        private DiagramToolWindow CreateToolWindow()
        {
            var toolWindow = CreateToolWindow(typeof(DiagramToolWindow), SingletonToolWindowInstanceId) as DiagramToolWindow;
            if (toolWindow?.Frame == null)
                throw new NotSupportedException("Cannot create tool window.");
            return toolWindow;
        }

        private void InitializeShellTriggeredCommands()
        {
            foreach (var commandType in DiscoverShellTriggeredCommandTypes())
            {
                var command = (ShellTriggeredCommandBase)Activator.CreateInstance(commandType, this);
                AddMenuCommand(this, command);
                _shellTriggeredCommands.Add(command);
            }
        }

        private static IEnumerable<Type> DiscoverShellTriggeredCommandTypes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.DefinedTypes.Where(i => i.IsSubclassOf(typeof(ShellTriggeredCommandBase)) && !i.IsAbstract);
        }

        private static void AddMenuCommand(IServiceProvider serviceProvider, ShellTriggeredCommandBase command)
        {
            var commandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
                throw new Exception("Unable to get IMenuCommandService.");

            var menuCommandId = new CommandID(command.CommandSet, command.CommandId);
            var menuItem = new OleMenuCommand(command.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        private void InitializeEventTriggeredCommands()
        {
            foreach (var commandType in DiscoverEventTriggeredCommandTypes())
            {
                var eventArgumentType = commandType.BaseType?.GenericTypeArguments.FirstOrDefault();
                if (eventArgumentType == null)
                    throw new Exception($"{commandType.Name} should have 1 type argument.");

                var command = (CommandBase)Activator.CreateInstance(commandType, this);
                _eventTriggeredCommands.Add(eventArgumentType, command);
            }
        }

        private static IEnumerable<Type> DiscoverEventTriggeredCommandTypes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.DefinedTypes
                .Where(i => i.BaseType != null
                            && i.BaseType.IsConstructedGenericType
                            && i.BaseType.GetGenericTypeDefinition() == typeof(EventTriggeredCommandBase<>)
                            && !i.IsAbstract);
        }

        private void OnPackageEvent(object sender, EventArgs eventArgs)
        {
            var command = _eventTriggeredCommands[eventArgs.GetType()];
            command?.Execute(sender, eventArgs);
        }
    }
}
