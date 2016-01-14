using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for PanAndZoomControl2.xaml
    /// </summary>
    public partial class PanAndZoomControl2 : UserControl
    {
        private const double WidthPerHeightRatio = 3d / 10d;

        private static readonly Brush FillDefault = Brushes.Transparent;
        private static readonly Brush StrokeDefault = Brushes.Black;
        private const double MinZoomDefault = .1d;
        private const double MaxZoomDefault = 10d;
        private const double ZoomValueDefault = 1d;
        private const double ZoomIncrementDefault = .5d;
        private const double PanAmountDefault = 50d;

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(FillDefault));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(StrokeDefault));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(MinZoomDefault));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(MaxZoomDefault));

        public static readonly DependencyProperty ZoomIncrementProperty =
            DependencyProperty.Register("ZoomIncrement", typeof(double), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(ZoomIncrementDefault));

        public static readonly DependencyProperty PanAmountProperty =
            DependencyProperty.Register("PanAmount", typeof(double), typeof(PanAndZoomControl2),
                new FrameworkPropertyMetadata(PanAmountDefault));

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomControl2),
                new PropertyMetadata(ZoomValueDefault));

        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register("ZoomCommand", typeof(ICommand), typeof(PanAndZoomControl2));

        public static readonly DependencyProperty DirectionPanCommandProperty =
            DependencyProperty.Register("DirectionPanCommand", typeof(ICommand), typeof(PanAndZoomControl2));

        public static readonly DependencyProperty VectorPanCommandProperty =
            DependencyProperty.Register("VectorPanCommand", typeof(ICommand), typeof(PanAndZoomControl2));

        public static readonly DependencyProperty CenterCommandProperty =
            DependencyProperty.Register("CenterCommand", typeof(ICommand), typeof(PanAndZoomControl2));

        public PanAndZoomControl2()
        {
            InitializeComponent();

            DirectionPanCommand = new DelegateCommand(i => VectorPanCommand?.Execute(CalculatePanVector((PanDirection)i)));
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
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

        public ICommand ZoomCommand
        {
            get { return (ICommand)GetValue(ZoomCommandProperty); }
            set { SetValue(ZoomCommandProperty, value); }
        }

        public ICommand DirectionPanCommand
        {
            get { return (ICommand)GetValue(DirectionPanCommandProperty); }
            set { SetValue(DirectionPanCommandProperty, value); }
        }

        public ICommand VectorPanCommand
        {
            get { return (ICommand)GetValue(VectorPanCommandProperty); }
            set { SetValue(VectorPanCommandProperty, value); }
        }

        public ICommand CenterCommand
        {
            get { return (ICommand)GetValue(CenterCommandProperty); }
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
    }
}
