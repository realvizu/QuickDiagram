using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramShapeFactory : IDiagramShapeFactory
    {
        public IDiagramNode CreateDiagramNode(IModelNode modelNode)
        {
            if (modelNode == null)
                throw new ArgumentNullException(nameof(modelNode));

            if (modelNode is TypeNode testType)
                return new TypeDiagramNode(testType);

            throw new ArgumentException($"Expected {typeof(TypeNode).Name} but received {modelNode.GetType().Name}");
        }
    }
}