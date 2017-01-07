using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for PanAndZoomControl.xaml
    /// </summary>
    public partial class PanAndZoomControl : UserControl
    {
        private const double WidthPerHeightRatio = 3d / 10d;

        private const double ZoomValueDefault = 1d;
        private const double ZoomIncrementDefault = .5d;
        private const double PanAmountDefault = 50d;

        public static readonly DependencyProperty FillProperty =
            Shape.FillProperty.AddOwner(typeof(PanAndZoomControl));

        public static readonly DependencyProperty MinZoomProperty =
            ZoomableVisual.MinZoomProperty.AddOwner(typeof(PanAndZoomControl));

        public static readonly DependencyProperty MaxZoomProperty =
            ZoomableVisual.MaxZoomProperty.AddOwner(typeof(PanAndZoomControl));

        public static readonly DependencyProperty ZoomIncrementProperty =
            DependencyProperty.Register("ZoomIncrement", typeof(double), typeof(PanAndZoomControl),
                new FrameworkPropertyMetadata(ZoomIncrementDefault));

        public static readonly DependencyProperty PanAmountProperty =
            DependencyProperty.Register("PanAmount", typeof(double), typeof(PanAndZoomControl),
                new FrameworkPropertyMetadata(PanAmountDefault));

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomControl),
                new PropertyMetadata(ZoomValueDefault));

        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register("ZoomCommand", typeof(DoubleDelegateCommand), typeof(PanAndZoomControl));

        public static readonly DependencyProperty DirectionPanCommandProperty =
            DependencyProperty.Register("DirectionPanCommand", typeof(PanDirectionDelegateCommand), typeof(PanAndZoomControl));

        public static readonly DependencyProperty VectorPanCommandProperty =
            DependencyProperty.Register("VectorPanCommand", typeof(VectorDelegateCommand), typeof(PanAndZoomControl));

        public static readonly DependencyProperty CenterCommandProperty =
            DependencyProperty.Register("CenterCommand", typeof(DelegateCommand), typeof(PanAndZoomControl));

        public PanAndZoomControl()
        {
            DirectionPanCommand = new PanDirectionDelegateCommand(OnDirectionPan);

            InitializeComponent();
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

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

        public double ZoomIncrement
        {
            get { return (double)GetValue(ZoomIncrementProperty); }
            set { SetValue(ZoomIncrementProperty, value); }
        }

        public double PanAmount
        {
            get { return (double)GetValue(PanAmountProperty); }
            set { SetValue(PanAmountProperty, value); }
        }

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        public DoubleDelegateCommand ZoomCommand
        {
            get { return (DoubleDelegateCommand)GetValue(ZoomCommandProperty); }
            set { SetValue(ZoomCommandProperty, value); }
        }

        public PanDirectionDelegateCommand DirectionPanCommand
        {
            get { return (PanDirectionDelegateCommand)GetValue(DirectionPanCommandProperty); }
            set { SetValue(DirectionPanCommandProperty, value); }
        }

        public VectorDelegateCommand VectorPanCommand
        {
            get { return (VectorDelegateCommand)GetValue(VectorPanCommandProperty); }
            set { SetValue(VectorPanCommandProperty, value); }
        }

        public DelegateCommand CenterCommand
        {
            get { return (DelegateCommand)GetValue(CenterCommandProperty); }
            set { SetValue(CenterCommandProperty, value); }
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

        private Vector CalculatePanVector(PanDirection panDirection)
        {
            Vector vector;
            switch (panDirection)
            {
                case PanDirection.Up:
                    vector = new Vector(0, -PanAmount);
                    break;
                case PanDirection.Down:
                    vector = new Vector(0, PanAmount);
                    break;
                case PanDirection.Left:
                    vector = new Vector(-PanAmount, 0);
                    break;
                case PanDirection.Right:
                    vector = new Vector(PanAmount, 0);
                    break;
                default:
                    throw new Exception($"Unexpected PanDirection: {panDirection}");
            }
            return vector;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ZoomCommand?.Execute(e.NewValue);
        }

        private void OnDirectionPan(PanDirection panDirection)
        {
            var panVector = CalculatePanVector(panDirection);
            VectorPanCommand?.Execute(panVector);
        }
    }
}
