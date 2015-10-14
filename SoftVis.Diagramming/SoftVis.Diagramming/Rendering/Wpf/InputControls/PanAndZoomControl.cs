using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.InputControls
{
    /// <summary>
    /// A widget that the user can use to pan, zoom and fit-to-view some content.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The ZoomValue is communicated to the outside world on an exponential scale.
    /// That is, a movement on the zoom knob gives a small ZoomValue change at the lower end of the scale, 
    /// and gives larger and larger change as the knob moves towards the higher end of the scale.
    /// </remarks>
    [TemplatePart(Name = PART_PanUpRepeatButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_PanLeftRepeatButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_PanRightRepeatButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_PanDownRepeatButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_CenterButton, Type = typeof(Button))]
    internal partial class PanAndZoomControl : TemplatedControlBase
    {
        private const string PART_PanUpRepeatButton = "PART_PanUpRepeatButton";
        private const string PART_PanLeftRepeatButton = "PART_PanLeftRepeatButton";
        private const string PART_PanRightRepeatButton = "PART_PanRightRepeatButton";
        private const string PART_PanDownRepeatButton = "PART_PanDownRepeatButton";
        private const string PART_CenterButton = "PART_CenterButton";
        private const double WidthPerHeightRatio = 3d / 10d;
        private const double SmallIncrementRatio = 1d / 12d;
        private const double LargeIncrementRatio = 1d / 6d;

        static PanAndZoomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanAndZoomControl), new FrameworkPropertyMetadata(typeof(PanAndZoomControl)));
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;
        public event EventHandler FitToView;

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        public double LinearZoomValue
        {
            get { return (double)GetValue(LinearZoomValueProperty); }
            set { SetValue(LinearZoomValueProperty, value); }
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

            SmallIncrement = (MaxZoom - MinZoom) * SmallIncrementRatio;
            LargeIncrement = (MaxZoom - MinZoom) * LargeIncrementRatio;

            FindChildControlInTemplate<RepeatButton>(PART_PanUpRepeatButton).Click += OnPanUp;
            FindChildControlInTemplate<RepeatButton>(PART_PanLeftRepeatButton).Click += OnPanLeft;
            FindChildControlInTemplate<RepeatButton>(PART_PanRightRepeatButton).Click += OnPanRight;
            FindChildControlInTemplate<RepeatButton>(PART_PanDownRepeatButton).Click += OnPanDown;
            FindChildControlInTemplate<Button>(PART_CenterButton).Click += OnFitToView;
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

        private static Size SizeToRatio(Size size)
        {
            var calculatedHeight = size.Width / WidthPerHeightRatio;
            var calculatedWidth = size.Height * WidthPerHeightRatio;

            var width = Math.Min(calculatedWidth, size.Width);
            var height = Math.Min(calculatedHeight, size.Height);

            return new Size(width, height);
        }

        private static void ZoomValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (double)e.OldValue;
            var newValue = (double)e.NewValue;

            if (oldValue.IsEqualWithTolerance(newValue))
                return;

            var panAndZoomControl = (PanAndZoomControl)obj;
            var linearZoomValue = ScaleCalculator.ExponentialToLinear(newValue, panAndZoomControl.MinZoom, panAndZoomControl.MaxZoom);
            panAndZoomControl.LinearZoomValue = linearZoomValue;

            panAndZoomControl.Zoom?.Invoke(panAndZoomControl, new ZoomEventArgs(newValue));
        }

        private static void LinearZoomValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (double)e.OldValue;
            var newValue = (double)e.NewValue;

            if (oldValue.IsEqualWithTolerance(newValue))
                return;

            var panAndZoomControl = (PanAndZoomControl)obj;
            var exponentialZoomValue = ScaleCalculator.LinearToExponential(newValue, panAndZoomControl.MinZoom, panAndZoomControl.MaxZoom);
            panAndZoomControl.ZoomValue = exponentialZoomValue;
        }

        private void OnPanUp(object sender, RoutedEventArgs e)
        {
            RaisePanEvent(PanDirection.Up);
        }

        private void OnPanLeft(object sender, RoutedEventArgs e)
        {
            RaisePanEvent(PanDirection.Left);
        }

        private void OnPanRight(object sender, RoutedEventArgs e)
        {
            RaisePanEvent(PanDirection.Right);
        }

        private void OnPanDown(object sender, RoutedEventArgs e)
        {
            RaisePanEvent(PanDirection.Down);
        }

        private void RaisePanEvent(PanDirection direction)
        {
            Pan?.Invoke(this, new PanEventArgs(direction, 1));
        }

        private void OnFitToView(object sender, RoutedEventArgs e)
        {
            FitToView?.Invoke(this, EventArgs.Empty);
        }
    }
}
