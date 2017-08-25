using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramStore : ModelTrackingDiagramStore, IDiagramServices
    {
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap = 
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
        {
            {ModelRelationshipStereotypes.Inheritance, RoslynConnectorTypes.Generalization},
            {ModelRelationshipStereotypes.Implementation, RoslynConnectorTypes.Implementation},
        };

        public RoslynDiagramStore(IReadOnlyModelStore modelStore, IDiagramShapeFactory diagramShapeFactory, Diagram diagram)
            : base(modelStore, diagramShapeFactory, diagram)
        {
        }

        public override ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                throw new Exception($"Unexpected model relationship type {stereotype.Name}");

            return ModelRelationshipTypeToConnectorTypeMap[stereotype];
        }

        public IDiagramNode ShowModelNode(IRoslynModelNode modelNode)
        {
            return ShowModelItem(modelNode) as IDiagramNode;
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IRoslynModelNode> modelNodes, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return ShowModelItems(modelNodes, cancellationToken, progress).OfType<IDiagramNode>().ToArray();
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var baseTypes = _modelServices.CurrentModel.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.BaseType, recursive: true);
            var subtypes = _modelServices.CurrentModel.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.Subtype, recursive: true);
            var entities = new[] { modelNode }.Union(baseTypes).Union(subtypes);

            return ShowModelItems(entities, cancellationToken, progress).OfType<IDiagramNode>().ToArray();
        }

        public void UpdateFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            foreach (var diagramNode in Nodes)
            {
                _modelServices.ExtendModelWithRelatedEntities(diagramNode.ModelNode, cancellationToken: cancellationToken);
                progress?.Report(1);
            }
        }
    }
}
