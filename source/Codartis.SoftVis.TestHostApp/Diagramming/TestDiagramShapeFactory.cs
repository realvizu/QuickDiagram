using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramShapeFactory : DiagramShapeFactoryBase
    {
        public override IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode, IModelNode parentModelNode)
        {
            if (modelNode == null)
                throw new ArgumentNullException(nameof(modelNode));

            if (modelNode is TypeNode typeNode)
                return new TypeDiagramNode(typeNode);

            if (modelNode is PropertyNode propertyNode)
            {
                var parentDiagramNode = diagramShapeResolver.GetDiagramNode(parentModelNode.Id) as TypeDiagramNode;
                return new PropertyDiagramNode(propertyNode, parentDiagramNode);
            }

            throw new ArgumentException($"Unexpected type {modelNode.GetType().Name} in {GetType().Name}");
        }
    }
}