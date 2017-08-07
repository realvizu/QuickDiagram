using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.TestHostApp.TestData;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramBuilder : DiagramBuilder
    {
        public override ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            switch (modelRelationship)
            {
                case TestInheritanceRelationship _:
                    return ConnectorTypes.Generalization;
                case TestImplementsRelationship _:
                    return TestConnectorTypes.Implementation;
                default:
                    throw new Exception($"Unexpected model relationship type {modelRelationship.GetType().Name}");
            }
        }
    }
}
