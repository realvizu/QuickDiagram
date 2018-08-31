using System;
using System.ComponentModel.Design;
using System.Diagnostics; // Do not remove, used in Release config.
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", PackageInfo.Version, IconResourceID = 401)]
    [Guid(PackageGuids.SoftVisPackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(DiagramHostToolWindow))]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SoftVisPackage : AsyncPackage, IPackageServices
    {
        static SoftVisPackage()
        {
            // HACK: Force load System.Windows.Interactivity.dll from plugin's directory.
            // See: http://stackoverflow.com/questions/29362125/visual-studio-extension-could-not-find-a-required-assembly
            typeof(System.Windows.Interactivity.Behavior).ToString();
        }

        private IComponentModel _componentModel;
        private DiagramToolApplication _diagramToolApplication;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            RegisterExceptionHandler();

            _componentModel = await GetServiceAsync<SComponentModel, IComponentModel>();

            var hostWorkspaceGateway = new HostWorkspaceGateway(this);
            await hostWorkspaceGateway.InitAsync();
            var hostUiGateway = new HostUiGateway(this);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _diagramToolApplication = new DiagramToolApplication(hostWorkspaceGateway, hostUiGateway);

            RegisterShellCommands(GetMenuCommandService(), _diagramToolApplication);
        }

        public void ShowToolWindow<TWindow>(int instanceId = 0) 
            where TWindow : ToolWindowPane
        {
            JoinableTaskFactory.Run(async () =>
            {
                await ShowToolWindowAsync(
                    typeof(DiagramHostToolWindow),
                    0,
                    create: true,
                    cancellationToken: DisposalToken);
            });
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(PackageGuids.DiagramToolWindowGuid) ? this : null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(DiagramHostToolWindow) ? DiagramHostToolWindow.WindowTitle : null;
        }

        protected override Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // Perform as much work as possible in this method which is being run on a background thread.
            // The object returned from this method is passed into the constructor of the SampleToolWindow 

            return Task.FromResult((object)_diagramToolApplication.ApplicationUiService.DiagramControl);
        }

        public void Await(Func<Task> action)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate {
                await action();
            });
        }

        private async Task<TInterface> GetServiceAsync<TService, TInterface>()
            where TService : class
            where TInterface : class
        {
            var service = await GetServiceAsync(typeof(TService));
            if (service == null)
                throw new Exception($"Unable to get {typeof(TService).FullName}.");
            if (!(service is TInterface))
                throw new Exception($"The requested service {typeof(TService).FullName} is not of type {typeof(TInterface).FullName}.");
            return service as TInterface;
        }

        private static void RegisterExceptionHandler()
        {
            // This is needed otherwise VS catches the exception and shows no stack trace.
            Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) =>
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#else
                Trace.WriteLine($"[{PackageInfo.ToolName}] unhandled exception: {args.Exception}");
#endif
            };
        }

        public DTE2 GetHostEnvironmentService()
        {
            var hostService = GetService(typeof(DTE)) as DTE2;
            if (hostService == null)
                throw new Exception("Unable to get DTE service.");
            return hostService;
        }

        public async Task<IVsRunningDocumentTable> GetRunningDocumentTableServiceAsync()
        {
            return await GetServiceAsync<SVsRunningDocumentTable, IVsRunningDocumentTable>();
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

        public async Task<TWindow> CreateToolWindowAsync<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            return await ShowToolWindowAsync(typeof(TWindow), instanceId, create: true, cancellationToken: DisposalToken) as TWindow;
        }

        private void RegisterShellCommands(IMenuCommandService menuCommandService, IAppServices appServices)
        {
            var commandSetGuid = PackageGuids.SoftVisCommandSetGuid;
            var commandRegistrant = new CommandRegistrant(menuCommandService, appServices);
            commandRegistrant.RegisterCommands(commandSetGuid, ShellCommands.CommandSpecifications);
            commandRegistrant.RegisterCombos(commandSetGuid, ShellCommands.ComboSpecifications);
        }
    }
}