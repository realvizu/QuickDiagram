using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl : UserControl, IDiagramStlyeProvider
    {
        private readonly ResourceDictionary _additionalResourceDictionary;

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DiagramViewportControl.PanAndZoomControlHeightProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty UndisplayedEntityCuePlacementDictionaryProperty =
            DependencyProperty.Register("UndisplayedEntityCuePlacementDictionary", typeof(IDictionary), typeof(DiagramControl));

        public static readonly DependencyProperty DiagramShapeButtonPlacementDictionaryProperty =
            DependencyProperty.Register("DiagramShapeButtonPlacementDictionary", typeof(IDictionary), typeof(DiagramControl));

        public DiagramControl() : this(null)
        { }

        public DiagramControl(ResourceDictionary additionalResourceDictionary = null)
        {
            _additionalResourceDictionary = additionalResourceDictionary;

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

        public double PanAndZoomControlHeight
        {
            get { return (double)GetValue(PanAndZoomControlHeightProperty); }
            set { SetValue(PanAndZoomControlHeightProperty, value); }
        }

        public IDictionary UndisplayedEntityCuePlacementDictionary
        {
            get { return (IDictionary)GetValue(UndisplayedEntityCuePlacementDictionaryProperty); }
            set { SetValue(UndisplayedEntityCuePlacementDictionaryProperty, value); }
        }

        public IDictionary DiagramShapeButtonPlacementDictionary
        {
            get { return (IDictionary)GetValue(DiagramShapeButtonPlacementDictionaryProperty); }
            set { SetValue(DiagramShapeButtonPlacementDictionaryProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (_additionalResourceDictionary != null)
                this.AddResourceDictionary(_additionalResourceDictionary);

            base.OnApplyTemplate();
        }
    }
}
