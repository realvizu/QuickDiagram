using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Specializes the diagram class for the VS integrated usage.
    /// </summary>
    internal class RoslynBasedDiagram : AutoArrangingDiagram, IDiagramServices
    {
        private readonly IModelServices _modelServices;

        public event Action<List<IModelItem>> ShowItemsRequested;

        public RoslynBasedDiagram(IModelServices modelServices)
            : base(modelServices.Model)
        {
            _modelServices = modelServices;
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

        public override void ShowItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = new CancellationToken(),
            IIncrementalProgress progress = null)
        {
            var modelItemList = modelItems.ToList();

            if (progress == null && modelItemList.Count > 1)
                ShowItemsRequested?.Invoke(modelItemList);
            else
                ShowItemsWithProgress(modelItemList, cancellationToken, progress);
        }

        public void ShowItemsWithProgress(IEnumerable<IModelItem> modelItems, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            base.ShowItems(modelItems, cancellationToken, progress);
        }

        public void ShowEntity(IRoslynBasedModelEntity modelEntity)
        {
            ShowItem(modelEntity);
        }

        public void ShowEntityWithHierarchy(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var baseTypes = Model.GetRelatedEntities(modelEntity, EntityRelationTypes.BaseType, recursive: true);
            var subtypes = Model.GetRelatedEntities(modelEntity, EntityRelationTypes.Subtype, recursive: true);

            var entities = new[] { modelEntity }.Union(baseTypes).Union(subtypes);
            ShowItems(entities, cancellationToken, progress);
        }

        public void UpdateFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            foreach (var diagramNode in Nodes.ToArray())
            {
                _modelServices.ExtendModelWithRelatedEntities(diagramNode.ModelEntity, cancellationToken: cancellationToken);
                progress?.Report(1);
            }
        }
    }
}
