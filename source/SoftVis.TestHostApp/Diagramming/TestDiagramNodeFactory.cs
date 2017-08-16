using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramNodeFactory : IDiagramNodeFactory
    {
        public IDiagramNode CreateDiagramNode(IModelNode modelNode)
        {
            if (modelNode == null)
                throw new ArgumentNullException(nameof(modelNode));

            if (modelNode is TypeNodeBase testType)
                return new TypeDiagramNode(testType);

            throw new ArgumentException($"Expected {typeof(TypeNodeBase).Name} but received {modelNode.GetType().Name}");
        }
    }
}