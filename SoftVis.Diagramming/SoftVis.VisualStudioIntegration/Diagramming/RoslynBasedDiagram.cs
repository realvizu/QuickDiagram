using System;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Specializes the diagram class for the VS integrated usage.
    /// </summary>
    internal class RoslynBasedDiagram : Diagram, IDiagramServices
    {
        private const int DiagramNodeMinWidth = 55;
        private const int DiagramNodeMinHeight = 50;

        public RoslynBasedDiagram() 
            : base(new RoslynBasedConnectorTypeResolver())
        {
        }

        public void ShowModelEntity(IRoslynBasedModelEntity modelEntity)
        {
            ShowItems(new[] { modelEntity });
        }

        public void ShowModelEntityWithRelatedEntities(IRoslynBasedModelEntity modelEntity)
        {
            throw new NotImplementedException();
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
