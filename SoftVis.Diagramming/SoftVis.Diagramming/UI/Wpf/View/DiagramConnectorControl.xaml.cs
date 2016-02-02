using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramConnectorControl.xaml.
    /// Animates RoutePoints changes.
    /// </summary>
    public partial class DiagramConnectorControl : UserControl
    {
        private static readonly IList<Point> EmptyPointList = new List<Point>();

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramConnectorControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramConnectorControl));

        public static readonly DependencyProperty AnimatedRoutePointsProperty =
            DependencyProperty.Register("AnimatedRoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(EmptyPointList, OnAnimatedRoutePointsChanged));

        public static readonly DependencyProperty RoutePointsProperty =
            DependencyProperty.Register("RoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(EmptyPointList,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        private static void OnAnimatedRoutePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramConnectorControl)d).OnAnimatedRoutePointsChanged((IList<Point>)e.OldValue, (IList<Point>)e.NewValue);

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(DiagramConnectorControl),
                new PropertyMetadata((Duration)TimeSpan.Zero));

        public DiagramConnectorControl()
        {
            InitializeComponent();
        }

        public Brush DiagramFill
        {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public Brush DiagramStroke
        {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
        }

        public IList<Point> RoutePoints
        {
            get { return (IList<Point>)GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }

        public IList<Point> AnimatedRoutePoints
        {
            get { return (IList<Point>)GetValue(AnimatedRoutePointsProperty); }
            set { SetValue(AnimatedRoutePointsProperty, value); }
        }

        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        private void OnAnimatedRoutePointsChanged(IList<Point> oldValue, IList<Point> newValue)
        {
            if (oldValue == null || oldValue.Count == 0)
                RoutePoints = newValue;
            else
                AnimateRoutePoints(oldValue, newValue);
        }

        private void AnimateRoutePoints(IList<Point> oldValue, IList<Point> newValue)
        {
            var animation = new PointArrayAnimation(oldValue.ToArray(), newValue.ToArray(), AnimationDuration);
            BeginAnimation(RoutePointsProperty, animation);
        }
    }
}
