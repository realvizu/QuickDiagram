using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal sealed class RoslynConnectorTypeResolver : IConnectorTypeResolver
    {
        [NotNull]
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap =
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
            {
                { ModelRelationshipStereotypes.Inheritance, RoslynConnectorTypes.Generalization },
                { ModelRelationshipStereotypes.Implementation, RoslynConnectorTypes.Implementation },
                { ModelRelationshipStereotypes.Association, RoslynConnectorTypes.Association },
            };

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                return ModelRelationshipTypeToConnectorTypeMap[stereotype];

            throw new Exception($"No connector type found for {stereotype}");
        }
    }
}