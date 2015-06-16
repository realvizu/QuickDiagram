using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Controls
{
    public class DiagramNodeControl : Control
    {
        static DiagramNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNodeControl), new FrameworkPropertyMetadata(typeof(DiagramNodeControl)));
        }

        public static readonly DependencyProperty DiagramNodeProperty =
            DependencyProperty.Register("DiagramNode", typeof(DiagramNode), typeof(DiagramNodeControl), new UIPropertyMetadata(null));

        public DiagramNode DiagramNode
        {
            get { return (DiagramNode)GetValue(DiagramNodeProperty); }
            set { SetValue(DiagramNodeProperty, value); }
        }

        public static readonly DependencyProperty NodeTypeProperty =
            DependencyProperty.Register("NodeType", typeof(string), typeof(DiagramNodeControl), new UIPropertyMetadata(null));

        public string NodeType
        {
            get { return (string)GetValue(NodeTypeProperty); }
            set { SetValue(NodeTypeProperty, value); }
        }
    }
}
