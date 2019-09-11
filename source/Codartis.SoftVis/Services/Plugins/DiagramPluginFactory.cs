using System;
using Codartis.SoftVis.Diagramming.Definition.Layout;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public class DiagramPluginFactory : IDiagramPluginFactory
    {
        private readonly INodeLayoutAlgorithm _nodeLayoutAlgorithm;

        public DiagramPluginFactory(INodeLayoutAlgorithm nodeLayoutAlgorithm)
        {
            _nodeLayoutAlgorithm = nodeLayoutAlgorithm;
        }

        public virtual IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == DiagramPluginId.AutoLayoutDiagramPlugin)
                return new BufferingAutoLayoutDiagramPlugin(_nodeLayoutAlgorithm);
            if (diagramPluginId == DiagramPluginId.ConnectorHandlerDiagramPlugin)
                return new ConnectorHandlerDiagramPlugin();
            if (diagramPluginId == DiagramPluginId.ModelTrackingDiagramPlugin)
                return new ModelTrackingDiagramPlugin();

            throw new InvalidOperationException($"Unexpected DiagramPluginId: {diagramPluginId.Name}");
        }
    }
}