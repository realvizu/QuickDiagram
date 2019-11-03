using System;
using System.ComponentModel.Design;
// ReSharper disable once RedundantUsingDirective
// Do not remove, used in Release config.
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autofac;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.DirectConnector;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Codartis.Util.UI.Wpf.Resources;
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
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";

        static SoftVisPackage()
        {
            // HACK: Force load System.Windows.Interactivity.dll from plugin's directory.
            // See: http://stackoverflow.com/questions/29362125/visual-studio-extension-could-not-find-a-required-assembly
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            typeof(System.Windows.Interactivity.Behavior).ToString();
        }

        private DiagramToolApplication _diagramToolApplication;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            RegisterExceptionHandler();

            await base.InitializeAsync(cancellationToken, progress);

            var container = CreateDependencyContainer();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _diagramToolApplication = container.Resolve<DiagramToolApplication>();

            RegisterShellCommands(GetMenuCommandService(), _diagramToolApplication);
        }

        public async Task ShowToolWindowAsync<TWindow>(int instanceId = 0) 
            where TWindow : ToolWindowPane
        {
                await ShowToolWindowAsync(
                    // TODO: use typeof(TWindow)
                    typeof(DiagramHostToolWindow),
                    0,
                    create: true,
                    cancellationToken: DisposalToken);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(PackageGuids.DiagramToolWindowGuid) ? this : null;
        }

        // TODO: Try to delete this. DiagramHostToolWindow sets its own caption. 
        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(DiagramHostToolWindow) ? DiagramHostToolWindow.WindowTitle : null;
        }

        protected override Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // Perform as much work as possible in this method which is being run on a background thread.
            // The object returned from this method is passed into the constructor of the ToolWindow.

            return Task.FromResult((object)_diagramToolApplication.ApplicationUiService.DiagramControl);
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
            return (IVsTextManager) await GetServiceAsync(typeof(SVsTextManager));
        }

        public async Task<IVsEditorAdaptersFactoryService> GetEditorAdaptersFactoryServiceAsync()
        {
            var componentModel = (IComponentModel) await GetServiceAsync(typeof(SComponentModel));
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

        private IContainer CreateDependencyContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ModelService>().SingleInstance().As<IModelService>();
            builder.RegisterType<RelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>();

            builder.RegisterType<DiagramService>().As<IDiagramService>();
            builder.RegisterType<RoslynConnectorTypeResolver>().As<IConnectorTypeResolver>();

            builder.RegisterType<DiagramShapeUiFactory>().As<IDiagramShapeUiFactory>();

            builder.RegisterType<RoslynDiagramViewModel>().As<DiagramViewModel>()
                .WithParameter("initialIsDescriptionVisible", true)
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d);

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>()
                .WithParameter("additionalResourceDictionary", resourceDictionary);

            builder.RegisterType<ApplicationUiService>().As<IUiService>();

            builder.RegisterType<DiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2)
                .As<IDiagramLayoutAlgorithm>();

            builder.RegisterType<LayoutPriorityProvider>().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>();

            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>();
            //builder.RegisterType<ModelExtenderDiagramPlugin>().As<IDiagramPlugin>();

            builder.RegisterType<VisualizationService>().SingleInstance().As<IVisualizationService>();

            builder.RegisterType<RoslynModelService>().As<IRoslynModelService>();

            var softVisPackage = new TypedParameter(typeof(SoftVisPackage), this);
            builder.RegisterType<HostWorkspaceGateway>().WithParameter(softVisPackage).As<IHostModelProvider>();
            builder.RegisterType<HostUiGateway>().WithParameter(softVisPackage).As<IHostUiServices>();

            builder.RegisterInstance(this).As<IVisualStudioServices>();

            builder.RegisterType<DiagramToolApplication>();

            return builder.Build();
        }
    }
}