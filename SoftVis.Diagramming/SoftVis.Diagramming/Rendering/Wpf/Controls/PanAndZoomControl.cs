using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Rendering.Wpf.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf.Controls
{
    [TemplatePart(Name = PART_Slider, Type = typeof(Slider))]
    public class PanAndZoomControl : Control
    {
        public const string PART_Slider = "PART_Slider";
        public const double WidthPerHeightRatio = 3d / 10d;

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

        private Slider _slider;

        static PanAndZoomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanAndZoomControl), new FrameworkPropertyMetadata(typeof(PanAndZoomControl)));
        }

        public PanAndZoomControl()
        {
            ZoomValue = 1;
        }

        public static readonly DependencyProperty ZoomValueProperty =
           DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomControl));

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _slider = GetTemplateChild(PART_Slider) as Slider;
            _slider.ValueChanged += OnSliderValueChanged;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Zoom != null)
            {
                Zoom(this, new ZoomEventArgs(e.NewValue / (_slider.Maximum - _slider.Minimum)));
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
