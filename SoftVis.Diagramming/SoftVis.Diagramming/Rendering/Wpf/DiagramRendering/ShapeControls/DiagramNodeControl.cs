using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    public class DiagramNodeControl : DiagramShapeControlBase
    {
        static DiagramNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeControl), 
                new FrameworkPropertyMetadata(typeof(DiagramNodeControl)));
        }
    }
}
