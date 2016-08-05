using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
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

        public RoslynBasedDiagram(IReadOnlyModel model)
            : base(model)
        {
        }

        public override IEnumerable<EntityRelationType> GetEntityRelationTypes()
        {
            foreach (var entityRelationType in base.GetEntityRelationTypes())
                yield return entityRelationType;

            yield return RoslynEntityRelationTypes.ImplementedInterface;
            yield return RoslynEntityRelationTypes.ImplementerType;
        }

        public override ConnectorType GetConnectorType(ModelRelationshipType type)
        {
            return type.Stereotype == ModelRelationshipStereotypes.Implementation
                ? RoslynBasedConnectorTypes.Implementation
                : ConnectorTypes.Generalization;
        }

        public void ShowModelEntity(IRoslynBasedModelEntity modelEntity)
        {
            ShowItem(modelEntity);
        }

        public void ShowModelEntityWithHierarchy(IRoslynBasedModelEntity modelEntity)
        {
            ShowItem(modelEntity);
            ShowItems(Model.GetRelatedEntities(modelEntity, EntityRelationTypes.BaseType, recursive: true));
            ShowItems(Model.GetRelatedEntities(modelEntity, EntityRelationTypes.Subtype, recursive: true));
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
