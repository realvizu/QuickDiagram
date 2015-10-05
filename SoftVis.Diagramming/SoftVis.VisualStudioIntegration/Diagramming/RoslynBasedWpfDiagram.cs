using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// A diagram that is created from Roslyn based model and rendered with WPF.
    /// </summary>
    internal class RoslynBasedWpfDiagram : WpfDiagram
    {
        public override void ShowNode(IModelEntity modelEntity)
        {
            base.ShowNode(modelEntity);

            foreach (var modelRelationship in modelEntity.OutgoingRelationships.Concat(modelEntity.IncomingRelationships))
            {
                if (NodeExists(modelRelationship.Source) &&
                    NodeExists(modelRelationship.Target))
                {
                    ShowConnector(modelRelationship);
                }
            }
        }
    }
}
