using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramShapeFactory : DiagramShapeFactoryBase
    {
        public override IDiagramNode CreateDiagramNode(IDiagramShapeResolver diagramShapeResolver, IModelNode modelNode)
        {
            if (modelNode is IRoslynTypeNode roslynTypeNode)
                return new RoslynTypeDiagramNode(roslynTypeNode);

            throw new ArgumentException($"Expected {typeof(IRoslynTypeNode).Name} but received {modelNode.GetType().Name}");
        }
    }
}