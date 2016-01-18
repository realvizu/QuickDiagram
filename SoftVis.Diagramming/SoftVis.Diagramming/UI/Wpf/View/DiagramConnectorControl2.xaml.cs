using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.UI.Wpf.Animations;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramConnectorControl2.xaml.
    /// Animates RoutePoints changes.
    /// </summary>
    public partial class DiagramConnectorControl2 : UserControl
    {
        private static readonly Duration AnimationDurationDefault = TimeSpan.FromMilliseconds(250);

        public static readonly DependencyProperty RoutePointsProperty =
            DependencyProperty.Register("RoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl2),
                new FrameworkPropertyMetadata(null, OnRoutePointChanged));

        public static readonly DependencyProperty AnimatedRoutePointsProperty =
            DependencyProperty.Register("AnimatedRoutePoints", typeof(IList<Point>), typeof(DiagramConnectorControl2),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        private static void OnRoutePointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramConnectorControl2)d).OnRoutePointsChanged((IList<Point>)e.OldValue, (IList<Point>)e.NewValue);

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(DiagramConnectorControl2),
                new PropertyMetadata(AnimationDurationDefault));

        public static readonly DependencyProperty BoundingRectTopProperty =
            DependencyProperty.Register("BoundingRectTop", typeof(double), typeof(DiagramConnectorControl2),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty BoundingRectLeftProperty =
            DependencyProperty.Register("BoundingRectLeft", typeof(double), typeof(DiagramConnectorControl2),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public DiagramConnectorControl2()
        {
            InitializeComponent();
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

        public double BoundingRectTop
        {
            get { return (double)GetValue(BoundingRectTopProperty); }
            set { SetValue(BoundingRectTopProperty, value); }
        }

        public double BoundingRectLeft
        {
            get { return (double)GetValue(BoundingRectLeftProperty); }
            set { SetValue(BoundingRectLeftProperty, value); }
        }

        private void OnRoutePointsChanged(IList<Point> oldValue, IList<Point> newValue)
        {
            if (oldValue == null)
            {
                AnimatedRoutePoints = newValue;
            }
            else
            {
                var animation = new PointArrayAnimation(oldValue.ToArray(), newValue.ToArray(), AnimationDuration);
                BeginAnimation(AnimatedRoutePointsProperty, animation);
            }
        }
    }
}
