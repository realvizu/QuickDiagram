using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public static class DiagramConnectorControlFactory
    {
        public static DiagramConnectorControl CreateFrom(DiagramConnector diagramConnector, IDictionary<DiagramNode, DiagramNodeControl> diagramNodeControls)
        {
            return new DiagramConnectorControl
            {
                DiagramConnector = diagramConnector,
                Source = diagramNodeControls[diagramConnector.Source],
                Target = diagramNodeControls[diagramConnector.Target],
                Rect = diagramConnector.Rect.ToWpf(),
                DataContext = diagramConnector,
            };
        }
    }
}
