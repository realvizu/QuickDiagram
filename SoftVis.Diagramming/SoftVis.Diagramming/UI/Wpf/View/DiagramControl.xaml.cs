﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.Common;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl : UserControl
    {
        private const double MinZoomDefault = .2d;
        private const double MaxZoomDefault = 5d;
        private const double InitialZoomDefault = 1d;
        private const double PanAndZoomControlSizeDefault = 120d;

        private readonly ResourceDictionary _additionalResourceDictionary;

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramControl),
                new FrameworkPropertyMetadata(MinZoomDefault));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramControl),
                new FrameworkPropertyMetadata(MaxZoomDefault));

        public static readonly DependencyProperty InitialZoomProperty =
            DependencyProperty.Register("InitialZoom", typeof(double), typeof(DiagramControl),
                new PropertyMetadata(InitialZoomDefault));

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DependencyProperty.Register("PanAndZoomControlHeight", typeof(double), typeof(DiagramControl),
                new PropertyMetadata(PanAndZoomControlSizeDefault));

        public static readonly DependencyProperty FitContentCommandProperty =
            DependencyProperty.Register("FitContentCommand", typeof(ICommand), typeof(DiagramControl));

        public DiagramControl()
        {
            InitializeComponent();
        }

        public DiagramControl(ResourceDictionary additionalResourceDictionary) 
            : this()
        {
            _additionalResourceDictionary = additionalResourceDictionary;
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

        public double InitialZoom
        {
            get { return (double)GetValue(InitialZoomProperty); }
            set { SetValue(InitialZoomProperty, value); }
        }

        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
        }

        public ICommand FitContentCommand
        {
            get { return (ICommand)GetValue(FitContentCommandProperty); }
            set { SetValue(FitContentCommandProperty, value); }
        }

        public void FitContent()
        {
            FitContentCommand?.Execute(null);
        }

        public override void OnApplyTemplate()
        {
            if (_additionalResourceDictionary != null)
                this.AddResourceDictionary(_additionalResourceDictionary);

            base.OnApplyTemplate();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(DiagramViewportControl);
        }
    }
}
