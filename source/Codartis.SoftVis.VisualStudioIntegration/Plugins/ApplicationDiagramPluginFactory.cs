using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.Services.Plugins;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the diagram plugin factory with application-specific plugin creation logic.
    /// </summary>
    internal class ApplicationDiagramPluginFactory : DiagramPluginFactory
    {
        public ApplicationDiagramPluginFactory(ILayoutPriorityProvider layoutPriorityProvider, IDiagramShapeFactory diagramShapeFactory)
            : base(layoutPriorityProvider, diagramShapeFactory)
        {
        }

        public override IDiagramPlugin Create(DiagramPluginId diagramPluginId)
        {
            if (diagramPluginId == ApplicationDiagramPluginId.ModelExtenderDiagramPlugin)
                return new ModelExtenderDiagramPlugin();

            return base.Create(diagramPluginId);
        }
    }
}
