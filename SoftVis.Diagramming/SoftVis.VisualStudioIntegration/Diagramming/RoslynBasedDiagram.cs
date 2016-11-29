using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Specializes the diagram class for the VS integrated usage.
    /// </summary>
    internal class RoslynBasedDiagram : AutoArrangingDiagram, IDiagramServices
    {
        public RoslynBasedDiagram(IModelServices modelService)
            : base(modelService.Model)
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

        public void ShowModelEntityWithHierarchy(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IProgress<int> progress)
        {
            ShowItem(modelEntity);

            var baseTypes = Model.GetRelatedEntities(modelEntity, EntityRelationTypes.BaseType, recursive: true);
            ShowItems(baseTypes, cancellationToken, progress);

            var subtypes = Model.GetRelatedEntities(modelEntity, EntityRelationTypes.Subtype, recursive: true);
            ShowItems(subtypes, cancellationToken, progress);
        }
    }
}
