using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramBuilder : DiagramBuilder
    {
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap = 
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
        {
            {RoslynModelRelationshipStereotype.Inheritance, ConnectorTypes.Generalization},
            {RoslynModelRelationshipStereotype.Implementation, RoslynConnectorTypes.Implementation},
        };

        public override ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                throw new Exception($"Unexpected model relationship type {stereotype.Name}");

            return ModelRelationshipTypeToConnectorTypeMap[stereotype];
        }
    }
}
