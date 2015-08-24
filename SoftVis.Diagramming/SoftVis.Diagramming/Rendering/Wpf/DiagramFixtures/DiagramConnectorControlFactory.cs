using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public static class DiagramConnectorControlFactory
    {
        public static DiagramConnectorControl CreateFrom(DiagramConnector diagramConnector,
            IDictionary<DiagramShape, DiagramShapeControlBase> diagramShapeControls)
        {
            return new DiagramConnectorControl
            {
                DiagramConnector = diagramConnector,
                Source = (DiagramNodeControl)diagramShapeControls[diagramConnector.Source],
                Target = (DiagramNodeControl)diagramShapeControls[diagramConnector.Target],
                Rect = diagramConnector.Rect.ToWpf(),
                DataContext = diagramConnector,
            };
        }
    }
}
