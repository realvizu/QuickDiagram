using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public static class DiagramNodeControlFactory
    {
        public static DiagramNodeControl CreateFrom(DiagramNode diagramNode)
        {
            return new DiagramNodeControl
            {
                DiagramNode = diagramNode,
                NodeType = diagramNode.GetType().ToString(),
                Rect = diagramNode.Rect.ToWpf(),
                DataContext = diagramNode,
            };
        }
    }
}
