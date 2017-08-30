using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramStore : DiagramStoreBase
    {
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap = 
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
        {
            {ModelRelationshipStereotypes.Inheritance, RoslynConnectorTypes.Generalization},
            {ModelRelationshipStereotypes.Implementation, RoslynConnectorTypes.Implementation},
        };

        public RoslynDiagramStore(Diagram diagram)
            : base(diagram)
        {
        }

        public override ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                throw new Exception($"Unexpected model relationship type {stereotype.Name}");

            return ModelRelationshipTypeToConnectorTypeMap[stereotype];
        }
    }
}
