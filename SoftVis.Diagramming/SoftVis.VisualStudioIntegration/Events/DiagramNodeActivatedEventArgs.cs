using System;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.VisualStudioIntegration.Events
{
    internal class DiagramNodeActivatedEventArgs : EventArgs
    {
        public IDiagramNode DiagramNode { get; }

        public DiagramNodeActivatedEventArgs(IDiagramNode diagramNode)
        {
            DiagramNode = diagramNode;
        }
    }
}
