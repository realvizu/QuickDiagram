using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.Rendering.Wpf.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf.Controls
{
    [TemplatePart(Name = PART_Slider, Type = typeof(Slider))]
    public class PanAndZoomControl : Control
    {
        private const string PART_Slider = "PART_Slider";
        private const double WidthPerHeightRatio = 3d / 10d;

        private Slider _slider;

        public static readonly DependencyProperty ZoomMinProperty =
            DependencyProperty.Register("ZoomMin", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty ZoomMaxProperty =
            DependencyProperty.Register("ZoomMax", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty SmallIncrementProperty =
            DependencyProperty.Register("SmallIncrement", typeof(double), typeof(PanAndZoomControl));

        public static readonly DependencyProperty LargeIncrementProperty =
            DependencyProperty.Register("LargeIncrement", typeof(double), typeof(PanAndZoomControl));

        public double ZoomMin
        {
            get { return (double)GetValue(ZoomMinProperty); }
            set { SetValue(ZoomMinProperty, value); }
        }

        public double ZoomMax
        {
            get { return (double)GetValue(ZoomMaxProperty); }
            set { SetValue(ZoomMaxProperty, value); }
        }

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set
            {
                SetValue(ZoomValueProperty, value);
                Debug.WriteLine("ZoomValue={0}", value);
            }
        }

        public double SmallIncrement
        {
            get { return (double)GetValue(SmallIncrementProperty); }
            set { SetValue(SmallIncrementProperty, value); }
        }

        public double LargeIncrement
        {
            get { return (double)GetValue(LargeIncrementProperty); }
            set { SetValue(LargeIncrementProperty, value); }
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

        static PanAndZoomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanAndZoomControl), new FrameworkPropertyMetadata(typeof(PanAndZoomControl)));
        }

        public PanAndZoomControl(double zoomMin, double zoomMax, double zoomInit)
        {
            ZoomMin = zoomMin;
            ZoomMax = zoomMax;
            ZoomValue = zoomInit;
            SmallIncrement = (zoomMax - zoomMin) / 10;
            LargeIncrement = (zoomMax - zoomMin) / 4;
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _slider = GetTemplateChild(PART_Slider) as Slider;
            _slider.ValueChanged += OnSliderValueChanged;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            e.Handled = true;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Zoom != null)
            {
                Zoom(this, new ZoomEventArgs(e.NewValue));
            }
            e.Handled = true;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint = SizeToRatio(constraint);

            base.MeasureOverride(constraint);

            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var childBounds = SizeToRatio(arrangeBounds);

            base.ArrangeOverride(childBounds);

            return arrangeBounds;
        }

        private Size SizeToRatio(Size size)
        {
            double calculatedHeight = size.Width / WidthPerHeightRatio;
            double calculatedWidth = size.Height * WidthPerHeightRatio;

            var width = Math.Min(calculatedWidth, size.Width);
            var height = Math.Min(calculatedHeight, size.Height);

            return new Size(width, height);
        }
    }
}
