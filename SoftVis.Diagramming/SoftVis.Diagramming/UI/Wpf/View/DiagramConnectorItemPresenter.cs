using System.Windows;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A special kind of ContentPresenter for diagram shape items. 
    /// </summary>
    public class DiagramConnectorItemPresenter : AnimatedRectContentPresenter
    {
        static DiagramConnectorItemPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramConnectorItemPresenter),
                new FrameworkPropertyMetadata(typeof(DiagramConnectorItemPresenter)));
        }

        public static readonly DependencyProperty SourceNodePresenterProperty =
            DependencyProperty.Register("SourceNodePresenter", typeof(AnimatedRectContentPresenter), typeof(DiagramConnectorItemPresenter));

        public static readonly DependencyProperty TargetNodePresenterProperty =
            DependencyProperty.Register("TargetNodePresenter", typeof(AnimatedRectContentPresenter), typeof(DiagramConnectorItemPresenter));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(DiagramConnectorItemPresenter));

        public AnimatedRectContentPresenter SourceNodePresenter
        {
            get { return (AnimatedRectContentPresenter)GetValue(SourceNodePresenterProperty); }
            set { SetValue(SourceNodePresenterProperty, value); }
        }

        public AnimatedRectContentPresenter TargetNodePresenter
        {
            get { return (AnimatedRectContentPresenter)GetValue(TargetNodePresenterProperty); }
            set { SetValue(TargetNodePresenterProperty, value); }
        }

        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        protected override void OnRectChanged(Rect oldValue, Rect newValue)
        {
            Center = newValue.GetCenter();
        }
    }
}
