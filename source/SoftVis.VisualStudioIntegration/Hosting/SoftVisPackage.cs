using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Debugger = System.Diagnostics.Debugger;
using IVisualStudioServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 401)]
    [Guid(PackageGuids.SoftVisPackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(DiagramHostToolWindow))]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class SoftVisPackage : Package, IPackageServices
    {
        static SoftVisPackage()
        {
            // HACK: Force load System.Windows.Interactivity.dll from plugin's directory.
            // See: http://stackoverflow.com/questions/29362125/visual-studio-extension-could-not-find-a-required-assembly
            typeof(System.Windows.Interactivity.Behavior).ToString();
        }

        private IVisualStudioServiceProvider _visualStudioServiceProvider;
        private IComponentModel _componentModel;
        private DiagramToolApplication _diagramToolApplication;

        protected override void Initialize()
        {
            base.Initialize();

            // This is needed otherwise VS catches the exception and shows no stack trace.
            Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) =>
            {
#if DEBUG
                Debugger.Break();
#else
                return;
#endif
            };

            _visualStudioServiceProvider = GetGlobalService(typeof(IVisualStudioServiceProvider)) as IVisualStudioServiceProvider;
            if (_visualStudioServiceProvider == null)
                throw new Exception("Unable to get IVisualStudioServiceProvider.");

            _componentModel = GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (_componentModel == null)
                throw new Exception("Unable to get IComponentModel.");

            var hostWorkspaceGateway = new HostWorkspaceGateway(this);
            var hostUiGateway = new HostUiGateway(this);

            var modelServices = new RoslynBasedModelBuilder(hostWorkspaceGateway);
            var diagramServices = new RoslynBasedDiagram(modelServices);
            var uiServices = new DiagramUi(hostUiGateway, diagramServices);

            _diagramToolApplication = new DiagramToolApplication(modelServices, diagramServices, uiServices);

            RegisterShellCommands(GetMenuCommandService(), _diagramToolApplication);
        }

        public DTE2 GetHostEnvironmentService()
        {
            var hostService = GetService(typeof(DTE)) as DTE2;
            if (hostService == null)
                throw new Exception("Unable to get DTE service.");
            return hostService;
        }

        public IVsRunningDocumentTable GetRunningDocumentTableService()
        {
            return GetVisualStudioService<IVsRunningDocumentTable, SVsRunningDocumentTable>();
        }

        public OleMenuCommandService GetMenuCommandService()
        {
            var commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
                throw new Exception("Unable to get IMenuCommandService.");
            return commandService;
        }

        public VisualStudioWorkspace GetVisualStudioWorkspace()
        {
            return _componentModel.GetService<VisualStudioWorkspace>();
        }

        public TWindow CreateToolWindow<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            var toolWindow = CreateToolWindow(typeof(TWindow), instanceId) as TWindow;
            if (toolWindow?.Frame == null)
                throw new NotSupportedException("Cannot create tool window.");
            return toolWindow;
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

        private static void RegisterShellCommands(IMenuCommandService menuCommandService, IAppServices appServices)
        {
            var commandSetGuid = PackageGuids.SoftVisCommandSetGuid;
            var commandRegistrator = new CommandRegistrator(menuCommandService, appServices);
            commandRegistrator.RegisterCommands(commandSetGuid, ShellCommands.CommandSpecifications);
            commandRegistrator.RegisterCombos(commandSetGuid, ShellCommands.ComboSpecifications);
        }
    }
}
