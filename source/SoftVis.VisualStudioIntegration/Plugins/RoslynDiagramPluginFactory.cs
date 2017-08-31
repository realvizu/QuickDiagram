using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.Service.Plugins;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the diagram plugin factory with Rosly-specific plugin creation logic.
    /// </summary>
    internal class RoslynDiagramPluginFactory : DiagramPluginFactory
    {
        public RoslynDiagramPluginFactory(ILayoutPriorityProvider layoutPriorityProvider, IDiagramShapeFactory diagramShapeFactory)
            : base(layoutPriorityProvider, diagramShapeFactory)
        {
        }

        public override IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == RoslynDiagramPluginId.ModelExtenderDiagramPlugin)
                return new ModelExtenderDiagramPlugin();

            return base.Create(diagramPluginId);
        }
    }
}
