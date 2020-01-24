using System;
using System.ComponentModel.Design;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.UI;
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
    public sealed class SoftVisPackage : AsyncPackage, IPackageServices
    {
        static SoftVisPackage()
        {
            // HACK: Force load System.Windows.Interactivity.dll from plugin's directory.
            // See: http://stackoverflow.com/questions/29362125/visual-studio-extension-could-not-find-a-required-assembly
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            typeof(Microsoft.Xaml.Behaviors.Behavior).ToString();
        }

        // This is public static to work around the limitation that no ctor param can be passed to DiagramHostToolWindow.
        public static DiagramToolApplication DiagramToolApplication { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            RegisterExceptionHandler();

            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var hostWorkspaceGateway = new HostWorkspaceGateway(this);
            var hostUiGateway = new HostUiGateway(this);

            var modelServices = new RoslynBasedModelBuilder(hostWorkspaceGateway);
            var diagramServices = new RoslynBasedDiagram(modelServices);
            var uiServices = new DiagramUi(diagramServices);

            DiagramToolApplication = new DiagramToolApplication(modelServices, diagramServices, uiServices, hostUiGateway);

            await RegisterShellCommandsAsync(DiagramToolApplication);
        }

        public async Task<DiagramHostToolWindow> GetToolWindowAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var toolWindow = FindToolWindow(typeof(DiagramHostToolWindow), 0, create: true) as DiagramHostToolWindow;
            if (toolWindow?.Frame == null)
                throw new NotSupportedException("Cannot create tool window.");

            return toolWindow;
        }

        public async Task<DTE2> GetHostEnvironmentServiceAsync()
        {
            return await GetServiceAsync<DTE, DTE2>();
        }

        public async Task<IVsTextManager> GetTextManagerServiceAsync()
        {
            return await GetServiceAsync<SVsTextManager, IVsTextManager>();
        }

        public async Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync()
        {
            var componentModel = await GetComponentModelService();
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }

        public async Task<VisualStudioWorkspace> GetVisualStudioWorkspaceAsync()
        {
            var componentModel = await GetComponentModelService();
            return componentModel.GetService<VisualStudioWorkspace>() ?? throw new Exception("Cannot get VisualStudioWorkspace service.");
        }

        private async Task<IMenuCommandService> GetMenuCommandServiceAsync()
        {
            return await GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
        }

        private async Task<IComponentModel> GetComponentModelService()
        {
            return await GetServiceAsync<SComponentModel, IComponentModel>();
        }

        private async Task RegisterShellCommandsAsync(IAppServices appServices)
        {
            var menuCommandService = await GetMenuCommandServiceAsync();

            var commandSetGuid = PackageGuids.SoftVisCommandSetGuid;
            var commandRegistrator = new CommandRegistrator(menuCommandService, appServices);
            commandRegistrator.RegisterCommands(commandSetGuid, ShellCommands.CommandSpecifications);
            commandRegistrator.RegisterCombos(commandSetGuid, ShellCommands.ComboSpecifications);
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
                Trace.WriteLine($"[QuickDiagram] unhandled exception: {args.Exception}");
#endif
            };
        }
    }
}