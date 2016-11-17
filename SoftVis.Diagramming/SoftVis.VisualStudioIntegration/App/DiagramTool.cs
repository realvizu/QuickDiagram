using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands.ExplicitlyTriggered;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{ 
    /// <summary>
    /// The diagram tool application.
    /// </summary>
    /// <remarks>
    /// Sets up the model, the diagram, and the commands that implement the application logic.
    /// Provides application services to the commands.
    /// </remarks>
    public sealed class DiagramTool : IAppServices
    {
        private readonly IHostUiServices _hostUiServices;
        private readonly IHostWorkspaceServices _hostWorkspaceServices;
        private readonly DiagramUi _diagramUi;
        private readonly IModelServices _modelBuilder;
        private readonly RoslynBasedDiagram _diagram;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="hostUiServices">Provides host UI service.</param>
        /// <param name="hostWorkspaceServices">Provides host workspace services.</param>
        public DiagramTool(IHostUiServices hostUiServices, IHostWorkspaceServices hostWorkspaceServices)
        {
            _hostUiServices = hostUiServices;
            _hostWorkspaceServices = hostWorkspaceServices;

            var roslynModelProvider = new RoslynModelProvider(hostWorkspaceServices);
            _modelBuilder = new RoslynBasedModelBuilder(roslynModelProvider);
            
            _diagram = new RoslynBasedDiagram(_modelBuilder.Model);
            _diagramUi = new DiagramUi(_diagram);

            // The diagram control must be hosted in the window created by the host package.
            _hostUiServices.DiagramHostWindow.Initialize("Diagram", _diagramUi.ContentControl);

            // It is important to subscribe to diagram events after the DiagramUi is created 
            // because DiagramUi also subscribes to diagram events and must be executed before DiagramTool event handlers.
            SubscribeToDiagramEvents();

            RegisterShellTriggeredCommands(hostUiServices, this);
        }

        public IHostUiServices GetHostUiServices() => _hostUiServices;
        public IHostWorkspaceServices GetHostWorkspaceServices() => _hostWorkspaceServices;
        public IModelServices GetModelServices() => _modelBuilder;
        public IDiagramServices GetDiagramServices() => _diagram;
        public IUiServices GetUiServices() => _diagramUi;

        private void SubscribeToDiagramEvents()
        {
            _diagram.ShapeActivated += OnDiagramShapeActivated;
            _diagram.ShapeAdded += OnShapeAddedToDiagram;
        }

        /// <summary>
        /// When a shape is activated (eg. double-clicked) opens the corresponding source file.
        /// </summary>
        /// <param name="diagramShape"></param>
        private void OnDiagramShapeActivated(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            new ShowSourceFileCommand(this).Execute(diagramNode);
        }

        /// <summary>
        /// Whenever a shape is added to the diagram tries to expand the model with the related entities.
        /// </summary>
        /// <param name="diagramShape">The shape that was added to the diagram.</param>
        private void OnShapeAddedToDiagram(IDiagramShape diagramShape)
        {
            var roslynBasedModelEntity = diagramShape.ModelItem as RoslynBasedModelEntity;
            if (roslynBasedModelEntity == null)
                return;

            _modelBuilder.ExtendModelWithRelatedEntities(roslynBasedModelEntity);
        }

        private static void RegisterShellTriggeredCommands(IHostUiServices hostUiServices, IAppServices appServices)
        {
            foreach (var commandType in DiscoverShellTriggeredCommandTypes())
            {
                var command = (ShellTriggeredCommandBase)Activator.CreateInstance(commandType, appServices);
                hostUiServices.AddMenuCommand(command.CommandSet, command.CommandId, command.Execute);
            }
        }

        private static IEnumerable<Type> DiscoverShellTriggeredCommandTypes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.DefinedTypes.Where(i => i.IsSubclassOf(typeof(ShellTriggeredCommandBase)) && !i.IsAbstract);
        }
    }
}