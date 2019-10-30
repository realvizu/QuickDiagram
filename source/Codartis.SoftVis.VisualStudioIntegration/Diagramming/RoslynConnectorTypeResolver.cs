using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal sealed class RoslynConnectorTypeResolver : IConnectorTypeResolver
    {
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap =
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
            {
                { RoslynModelRelationshipStereotypes.Inheritance, RoslynConnectorTypes.Generalization },
                { RoslynModelRelationshipStereotypes.Implementation, RoslynConnectorTypes.Implementation },
                { RoslynModelRelationshipStereotypes.Association, RoslynConnectorTypes.Association },
            };

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (!ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                throw new Exception($"Unexpected model relationship type {stereotype}");

            return ModelRelationshipTypeToConnectorTypeMap[stereotype];
        }
    }
}