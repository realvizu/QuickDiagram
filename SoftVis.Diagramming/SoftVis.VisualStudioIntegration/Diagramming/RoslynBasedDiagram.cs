using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Specializes the diagram class for the VS integrated usage.
    /// </summary>
    internal class RoslynBasedDiagram : Diagram
    {
        private const int DiagramNodeMinWidth = 55;
        private const int DiagramNodeMinHeight = 50;

        public RoslynBasedDiagram(IConnectorTypeResolver connectorTypeResolver) 
            : base(connectorTypeResolver)
        {
        }

        protected override Size2D CalculateDiagramNodeSize(IModelEntity modelEntity)
        {
            var originalSize = base.CalculateDiagramNodeSize(modelEntity);
            var correctedWidth = Math.Max(originalSize.Width, DiagramNodeMinWidth);
            var correctedHeight = Math.Max(originalSize.Height, DiagramNodeMinHeight);
            return new Size2D(correctedWidth, correctedHeight);
        }
    }
}
