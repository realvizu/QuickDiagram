using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Nodes;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public class DiagramPluginFactory : IDiagramPluginFactory
    {
        private readonly ILayoutPriorityProvider _layoutPriorityProvider;
        private readonly IDiagramShapeFactory _diagramShapeFactory;
        private readonly INodeLayoutAlgorithm _nodeLayoutAlgorithm;

        public DiagramPluginFactory(ILayoutPriorityProvider layoutPriorityProvider, IDiagramShapeFactory diagramShapeFactory, INodeLayoutAlgorithm nodeLayoutAlgorithm)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
            _diagramShapeFactory = diagramShapeFactory;
            _nodeLayoutAlgorithm = nodeLayoutAlgorithm;
        }

        public virtual IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == DiagramPluginId.AutoLayoutDiagramPlugin) return new HierarchicalAutoLayoutPlugin(_layoutPriorityProvider);
            if (diagramPluginId == DiagramPluginId.AutoLayoutDiagramPlugin2) return new BufferingAutoLayoutDiagramPlugin(_nodeLayoutAlgorithm);
            if (diagramPluginId == DiagramPluginId.ConnectorHandlerDiagramPlugin) return new ConnectorHandlerDiagramPlugin(_diagramShapeFactory);
            if (diagramPluginId == DiagramPluginId.ModelTrackingDiagramPlugin) return new ModelTrackingDiagramPlugin(_diagramShapeFactory);

            throw new InvalidOperationException($"Unexpected DiagramPluginId: {diagramPluginId.Name}");
        }
    }
}
