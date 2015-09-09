using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// This control draws a diagram node on its parent canvas/panel.
    /// The visual appearance and the data bindings to its ViewModel are defined in XAML.
    /// </summary>
    public class DiagramNodeControl : DiagramShapeControlBase
    {
        static DiagramNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeControl), 
                new FrameworkPropertyMetadata(typeof(DiagramNodeControl)));
        }
    }
}
