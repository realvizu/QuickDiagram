using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynDiagramNodeFactory : IDiagramNodeFactory
    {
        public IDiagramNode CreateDiagramNode(IModelNode modelNode)
        {
            if (modelNode == null)
                throw new ArgumentNullException(nameof(modelNode));

            if (modelNode is IRoslynTypeNode roslynTypeNode)
                return new TypeDiagramNode(roslynTypeNode);

            throw new ArgumentException($"Expected {typeof(IRoslynTypeNode).Name} but received {modelNode.GetType().Name}");
        }
    }
}