using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Implements Roslyn-specific extensions to diagram operations.
    /// </summary>
    internal class RoslynDiagramService : DiagramService, IRoslynDiagramService
    {
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap =
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
            {
                {ModelRelationshipStereotypes.Inheritance, RoslynConnectorTypes.Generalization},
                {ModelRelationshipStereotypes.Implementation, RoslynConnectorTypes.Implementation},
            };

        public RoslynDiagramService(IDiagram diagram, IModelService modelService, IDiagramShapeFactory diagramShapeFactory) 
            : base(diagram, modelService,diagramShapeFactory)
        {
        }

        public override ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                throw new Exception($"Unexpected model relationship type {stereotype.Name}");

            return ModelRelationshipTypeToConnectorTypeMap[stereotype];
        }

        public IEnumerable<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var model = ModelService.Model;

            var baseTypes = model.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.BaseType, recursive: true);
            var subtypes = model.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.Subtype, recursive: true);
            var modelNodes = new[] { modelNode }.Union(baseTypes).Union(subtypes);

            return ShowModelNodes(modelNodes, cancellationToken, progress);
        }
    }
}
