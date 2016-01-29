using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.Commands;
using Codartis.SoftVis.UI.Wpf.Common;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl : UserControl
    {
        private readonly ResourceDictionary _additionalResourceDictionary;
        private readonly DiagramImageControl _diagramImageControl;
        private DiagramViewModel _diagramViewModel;

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DiagramViewportControl.PanAndZoomControlHeightProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty ExportImageCommandProperty =
            DependencyProperty.Register("ExportImageCommand", typeof(BitmapSourceDelegateCommand), typeof(DiagramControl));

        public DiagramControl()
        {
            _diagramImageControl = new DiagramImageControl();

            DataContextChanged += OnDataContextChanged;

            InitializeComponent();
        }

        public DiagramControl(ResourceDictionary additionalResourceDictionary)
            : this()
        {
            _additionalResourceDictionary = additionalResourceDictionary;
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

        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
        }

        public BitmapSourceDelegateCommand ExportImageCommand
        {
            get { return (BitmapSourceDelegateCommand)GetValue(ExportImageCommandProperty); }
            set { SetValue(ExportImageCommandProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (_additionalResourceDictionary != null)
                this.AddResourceDictionary(_additionalResourceDictionary);

            base.OnApplyTemplate();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(DataContext is DiagramViewModel))
                return;

            _diagramViewModel = (DiagramViewModel)DataContext;
            _diagramViewModel.DiagramImageExportRequested += OnDiagramImageExportRequested;

            _diagramImageControl.DataContext = DataContext;
        }

        private void OnDiagramImageExportRequested(double dpi)
        {
            UpdateDiagramImageControlProperties();

            var boundsTranslatedToOrigo = new Rect(_diagramViewModel.DiagramContentRect.Size);
            var bitmapSource = _diagramImageControl.GetImage(boundsTranslatedToOrigo, dpi);
            ExportImageCommand?.Execute(bitmapSource);
        }

        private void UpdateDiagramImageControlProperties()
        {
            _diagramImageControl.Background = Background;
            _diagramImageControl.Foreground = Foreground;
            _diagramImageControl.DiagramFill = DiagramFill;
            _diagramImageControl.DiagramStroke = DiagramStroke;
            _diagramImageControl.FontStyle = FontStyle;
            _diagramImageControl.FontSize = FontSize;
            _diagramImageControl.FontFamily = FontFamily;
            _diagramImageControl.FontStretch = FontStretch;
            _diagramImageControl.FontWeight = FontWeight;
        }
    }
}
