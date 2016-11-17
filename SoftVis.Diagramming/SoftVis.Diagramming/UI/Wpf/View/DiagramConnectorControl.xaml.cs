using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf.Animations;

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

        /// <summary>
        /// The original route points from the view model.
        /// </summary>
        public static readonly DependencyProperty RoutePointsToAnimateProperty =
            DependencyProperty.Register("RoutePointsToAnimate", typeof(IList<Point>), typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(EmptyPointList, OnRoutePointsToAnimateChanged));

        private static void OnRoutePointsToAnimateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramConnectorControl)d).OnRoutePointsToAnimateChanged((IList<Point>)e.OldValue, (IList<Point>)e.NewValue);

        /// <summary>
        /// Animated route points based on changes in the view mode.
        /// </summary>
        public static readonly DependencyProperty AnimatedRoutePointsProperty =
            DependencyProperty.Register("AnimatedRoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl));

        /// <summary>
        /// The displayed route points are the animated route points adjusted to follow the SourceNode and TargetNode.
        /// </summary>
        public static readonly DependencyProperty DisplayedRoutePointsProperty =
            DependencyProperty.Register("DisplayedRoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl),
                 new FrameworkPropertyMetadata(EmptyPointList,
                     FrameworkPropertyMetadataOptions.AffectsRender |
                     FrameworkPropertyMetadataOptions.AffectsMeasure |
                     FrameworkPropertyMetadataOptions.AffectsParentArrange));

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

        public IList<Point> RoutePointsToAnimate
        {
            get { return (IList<Point>)GetValue(RoutePointsToAnimateProperty); }
            set { SetValue(RoutePointsToAnimateProperty, value); }
        }

        public IList<Point> AnimatedRoutePoints
        {
            get { return (IList<Point>)GetValue(AnimatedRoutePointsProperty); }
            set { SetValue(AnimatedRoutePointsProperty, value); }
        }
        public IList<Point> DisplayedRoutePoints
        {
            get { return (IList<Point>)GetValue(DisplayedRoutePointsProperty); }
            set { SetValue(DisplayedRoutePointsProperty, value); }
        }

        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        private void OnRoutePointsToAnimateChanged(IList<Point> oldValue, IList<Point> newValue)
        {
            if (newValue == null || newValue.Count == 0)
                return;

            if (oldValue == null || oldValue.Count == 0)
                AnimatedRoutePoints = newValue;
            else
                AnimateRoutePoints(oldValue, newValue);
        }

        private void AnimateRoutePoints(IList<Point> oldValue, IList<Point> newValue)
        {
            var animation = new PointArrayAnimation(oldValue.ToArray(), newValue.ToArray(), AnimationDuration);
            BeginAnimation(AnimatedRoutePointsProperty, animation);
        }
    }
}
