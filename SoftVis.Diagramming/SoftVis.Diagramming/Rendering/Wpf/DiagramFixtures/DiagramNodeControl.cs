using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public class DiagramNodeControl : Control
    {
        static DiagramNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeControl), new FrameworkPropertyMetadata(typeof(DiagramNodeControl)));
        }

        public static readonly DependencyProperty DiagramNodeProperty =
            DependencyProperty.Register("DiagramNode", typeof(DiagramNode), typeof(DiagramNodeControl));

        public static readonly DependencyProperty NodeTypeProperty =
            DependencyProperty.Register("NodeType", typeof(string), typeof(DiagramNodeControl));

        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(DiagramNodeControl));

        public DiagramNode DiagramNode
        {
            get { return (DiagramNode)GetValue(DiagramNodeProperty); }
            set { SetValue(DiagramNodeProperty, value); }
        }

        public string NodeType
        {
            get { return (string)GetValue(NodeTypeProperty); }
            set { SetValue(NodeTypeProperty, value); }
        }

        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }
    }
}
