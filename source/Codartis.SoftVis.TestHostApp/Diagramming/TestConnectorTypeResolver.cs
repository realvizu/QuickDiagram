using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestConnectorTypeResolver : IConnectorTypeResolver
    {
        [NotNull]
        private static readonly Dictionary<ModelRelationshipStereotype, ConnectorType> ModelRelationshipTypeToConnectorTypeMap =
            new Dictionary<ModelRelationshipStereotype, ConnectorType>
            {
                { ModelRelationshipStereotypes.Inheritance, TestConnectorTypes.Generalization },
                { ModelRelationshipStereotypes.Implementation, TestConnectorTypes.Implementation },
                { ModelRelationshipStereotypes.Association, TestConnectorTypes.Association },
            };

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            if (ModelRelationshipTypeToConnectorTypeMap.ContainsKey(stereotype))
                return ModelRelationshipTypeToConnectorTypeMap[stereotype];

            throw new Exception($"No connector type found for {stereotype}");
        }
    }
}