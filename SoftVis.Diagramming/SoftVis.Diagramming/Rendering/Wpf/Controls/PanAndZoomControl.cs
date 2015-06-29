using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.Rendering.Wpf.Gestures;

namespace Codartis.SoftVis.Rendering.Wpf.Controls
{
    public partial class PanAndZoomControl : Control
    {
        private const double WidthPerHeightRatio = 3d / 10d;

        static PanAndZoomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanAndZoomControl), new FrameworkPropertyMetadata(typeof(PanAndZoomControl)));
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;

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
            set { SetValue(ZoomValueProperty, value); }
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SmallIncrement = (ZoomMax - ZoomMin) / 10;
            LargeIncrement = (ZoomMax - ZoomMin) / 5;
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

        private static void ZoomValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var panAndZoomControl = obj as PanAndZoomControl;
            if (panAndZoomControl != null &&
                panAndZoomControl.Zoom != null &&
                (double)e.OldValue != (double)e.NewValue)
            {
                panAndZoomControl.Zoom(panAndZoomControl, new ZoomEventArgs((double)e.NewValue));
            }
        }
    }
}
