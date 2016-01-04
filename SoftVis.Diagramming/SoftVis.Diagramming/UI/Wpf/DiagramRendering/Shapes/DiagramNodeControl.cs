using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes
{
    /// <summary>
    /// This control draws a diagram node on its parent canvas/panel.
    /// The visual appearance and the data bindings to its ViewModel are defined in XAML.
    /// </summary>
    public sealed class DiagramNodeControl : DiagramShapeControlBase
    {
        private readonly DiagramNode _diagramNode;

        static DiagramNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeControl), 
                new FrameworkPropertyMetadata(typeof(DiagramNodeControl)));
        }

        public DiagramNodeControl(DiagramNode diagramNode)
        {
            _diagramNode = diagramNode;
            DataContext = diagramNode;
        }

        protected override DiagramShape DiagramShape => _diagramNode;

        public override void RefreshBinding()
        {
            var rect = _diagramNode.Rect.ToWpf();
            Appear();
            MoveTo(rect.Location);
            SizeTo(rect.Size);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
