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
            var size = CalculateDiagramNodeSize(modelEntity);
            return new DiagramNodeViewModel(modelEntity, DiagramDefaults.DefaultNodePosition, size);
        }

        protected static Size2D CalculateDiagramNodeSize(IModelEntity modelEntity)
        {
            var atLeastMinimalWidth = Math.Max(modelEntity.Name.Length*10, DiagramDefaults.MinimumNodeWidth);
            var width = Math.Min(atLeastMinimalWidth, DiagramDefaults.MaximumNodeWidth);
            var size = new Size2D(width, DiagramDefaults.DefaultNodeHeight);
            return size;
        }

        protected override DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = (DiagramNodeViewModel)FindNode(relationship.Source);
            var targetNode = (DiagramNodeViewModel)FindNode(relationship.Target);
            return new DiagramConnectorViewModel(relationship, sourceNode, targetNode);
        }
    }
}
