using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    /// <summary>
    /// A diagram that can be rendered with WPF.
    /// </summary>
    public class WpfDiagram : Diagram
    {
        protected override DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            var width = Math.Min(Math.Max(modelEntity.Name.Length * 10, MinimumNodeWidth), MaximumNodeWidth);
            var size = new Size2D(width, DefaultNodeHeight);
            return new DiagramNodeViewModel(modelEntity, DefaultNodePosition, size);
        }

        protected override DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = (DiagramNodeViewModel)FindNode(relationship.Source);
            var targetNode = (DiagramNodeViewModel)FindNode(relationship.Target);
            return new DiagramConnectorViewModel(relationship, sourceNode, targetNode);
        }
    }
}
