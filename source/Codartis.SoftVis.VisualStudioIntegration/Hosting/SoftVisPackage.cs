using System;
using System.ComponentModel.Design;
// ReSharper disable once RedundantUsingDirective
// Do not remove, used in Release config.
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autofac;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
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
    [ProvideBindingPath]
    public sealed class SoftVisPackage : AsyncPackage, IVisualStudioServices
    {
        private DiagramToolApplication _diagramToolApplication;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            RegisterExceptionHandler();

            await base.InitializeAsync(cancellationToken, progress);

            var container = DependencyConfigurator.CreateDependencyContainer(this);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _diagramToolApplication = container.Resolve<DiagramToolApplication>();

            RegisterShellCommands(GetMenuCommandService(), _diagramToolApplication);
        }

        public async Task ShowToolWindowAsync<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            await ShowToolWindowAsync(
                typeof(TWindow),
                instanceId,
                create: true,
                cancellationToken: DisposalToken);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(PackageGuids.DiagramToolWindowGuid) ? this : null;
        }

        protected override Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // Perform as much work as possible in this method which is being run on a background thread.
            // The object returned from this method is passed into the constructor of the ToolWindow.

            return Task.FromResult((object)_diagramToolApplication.DiagramWindowService.DiagramControl);
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

        public async Task<IVsTextManager> GetTextManagerServiceAsync()
        {
            return (IVsTextManager)await GetServiceAsync(typeof(SVsTextManager));
        }

        public async Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync()
        {
            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
            if (componentModel == null)
                throw new ArgumentNullException(nameof(componentModel));

            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }

        public OleMenuCommandService GetMenuCommandService()
        {
            var commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
                throw new Exception("Unable to get IMenuCommandService.");

            return commandService;
        }

        public async Task<VisualStudioWorkspace> GetVisualStudioWorkspaceAsync()
        {
            var componentModel = await GetServiceAsync<SComponentModel, IComponentModel>();
            return componentModel.GetService<VisualStudioWorkspace>() ?? throw new Exception("Cannot get VisualStudioWorkspace service.");
        }

        public async Task<TWindow> CreateToolWindowAsync<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            return await ShowToolWindowAsync(typeof(TWindow), instanceId, create: true, cancellationToken: DisposalToken) as TWindow;
        }

        private static void RegisterShellCommands(IMenuCommandService menuCommandService, IAppServices appServices)
        {
            var commandSetGuid = PackageGuids.SoftVisCommandSetGuid;
            var commandRegistrant = new CommandRegistrant(menuCommandService, appServices);
            commandRegistrant.RegisterCommands(commandSetGuid, ShellCommands.CommandSpecifications);
            commandRegistrant.RegisterToggleCommands(commandSetGuid, ShellCommands.ToggleCommandSpecifications);
            commandRegistrant.RegisterCombos(commandSetGuid, ShellCommands.ComboSpecifications);
        }
    }
}