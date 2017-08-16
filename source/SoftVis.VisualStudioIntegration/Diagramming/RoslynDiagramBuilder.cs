using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramBuilder : DiagramBuilder
    {
        private static readonly Dictionary<Type, ConnectorType> ModelRelationshipTypeToConnectorTypeMap = new Dictionary<Type, ConnectorType>
        {
            {typeof(IInheritanceRelationship), ConnectorTypes.Generalization},
            {typeof(IImplementationRelationship), RoslynConnectorTypes.Implementation},
        };

        public override ConnectorType GetConnectorType(Type modelRelationshipType)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(modelRelationshipType))
                throw new Exception($"Unexpected model relationship type {modelRelationshipType.Name}");

            return ModelRelationshipTypeToConnectorTypeMap[modelRelationshipType];
        }
    }
}
