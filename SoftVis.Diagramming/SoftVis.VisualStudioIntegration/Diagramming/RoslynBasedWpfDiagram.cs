using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.DiagramRendering;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// A diagram that is created from Roslyn based model and rendered with WPF.
    /// </summary>
    internal class RoslynBasedWpfDiagram : WpfDiagram
    {
        public RoslynBasedWpfDiagram(IConnectorTypeResolver connectorTypeResolver) 
            : base(connectorTypeResolver)
        {
        }

        protected override void ShowEntity(IModelEntity modelEntity)
        {
            base.ShowEntity(modelEntity);

            foreach (var modelRelationship in modelEntity.AllRelationships)
            {
                if (NodeExists(modelRelationship.Source) &&
                    NodeExists(modelRelationship.Target))
                {
                    ShowRelationship(modelRelationship);
                }
            }
        }
    }
}
