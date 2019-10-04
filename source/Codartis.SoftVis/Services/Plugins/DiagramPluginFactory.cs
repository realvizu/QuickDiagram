using System;
using Codartis.SoftVis.Diagramming.Definition.Layout;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Creates diagram plugin instances.
    /// </summary>
    public class DiagramPluginFactory : IDiagramPluginFactory
    {
        private readonly IDiagramLayoutAlgorithm _diagramLayoutAlgorithm;

        public DiagramPluginFactory(IDiagramLayoutAlgorithm diagramLayoutAlgorithm)
        {
            _diagramLayoutAlgorithm = diagramLayoutAlgorithm;
        }

        public virtual IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == DiagramPluginId.AutoLayoutDiagramPlugin)
                return new AutoLayoutDiagramPlugin(_diagramLayoutAlgorithm);
            if (diagramPluginId == DiagramPluginId.ConnectorHandlerDiagramPlugin)
                return new ConnectorHandlerDiagramPlugin();
            if (diagramPluginId == DiagramPluginId.ModelTrackingDiagramPlugin)
                return new ModelTrackingDiagramPlugin();

            throw new InvalidOperationException($"Unexpected DiagramPluginId: {diagramPluginId.Name}");
        }
    }
}