using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.Service.Plugins
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public class DiagramPluginFactory : IDiagramPluginFactory
    {
        private readonly ILayoutPriorityProvider _layoutPriorityProvider;
        private readonly IDiagramShapeFactory _diagramShapeFactory;

        public DiagramPluginFactory(ILayoutPriorityProvider layoutPriorityProvider, IDiagramShapeFactory diagramShapeFactory)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
            _diagramShapeFactory = diagramShapeFactory;
        }

        public IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == DiagramPluginId.AutoLayoutDiagramPlugin) return new AutoLayoutDiagramPlugin(_layoutPriorityProvider);
            if (diagramPluginId == DiagramPluginId.ConnectorHandlerDiagramPlugin) return new ConnectorHandlerDiagramPlugin(_diagramShapeFactory);
            if (diagramPluginId == DiagramPluginId.ModelTrackingDiagramPlugin) return new ModelTrackingDiagramPlugin(_diagramShapeFactory);

            throw new InvalidOperationException($"Unexpected DiagramPluginId: {diagramPluginId.Name}");
        }
    }
}
