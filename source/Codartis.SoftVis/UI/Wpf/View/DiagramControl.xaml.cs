using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A DiagramControl graphically presents the contents of a DiagramViewModel using a DiagramCanvas.
    /// It is also interactive: nodes and connectors can be added/removed using mini buttons and other controls.
    /// The layout is dictated by the node and connector view models.
    /// </summary>
    public partial class DiagramControl : UserControl, IDiagramStyleProvider
    {
        public ResourceDictionary AdditionalResourceDictionary { get; }

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty PanAndZoomControlHeightProperty =
            DiagramViewportControl.PanAndZoomControlHeightProperty.AddOwner(typeof(DiagramControl));

        public static readonly DependencyProperty RelatedNodeCuePlacementDictionaryProperty =
            DependencyProperty.Register("RelatedNodeCuePlacementDictionary", typeof(IDictionary), typeof(DiagramControl));

        public static readonly DependencyProperty MiniButtonPlacementDictionaryProperty =
            DependencyProperty.Register("MiniButtonPlacementDictionary", typeof(IDictionary), typeof(DiagramControl));

        public DiagramControl()
            : this(null)
        {
        }

        public DiagramControl(ResourceDictionary additionalResourceDictionary = null)
        {
            AdditionalResourceDictionary = additionalResourceDictionary;

            InitializeComponent();
        }

        public EdgeMode EdgeMode => (EdgeMode)GetValue(RenderOptions.EdgeModeProperty);
        public ClearTypeHint ClearTypeHint => (ClearTypeHint)GetValue(RenderOptions.ClearTypeHintProperty);
        public TextRenderingMode TextRenderingMode => (TextRenderingMode)GetValue(TextOptions.TextRenderingModeProperty);
        public TextHintingMode TextHintingMode => (TextHintingMode)GetValue(TextOptions.TextHintingModeProperty);
        public TextFormattingMode TextFormattingMode => (TextFormattingMode)GetValue(TextOptions.TextFormattingModeProperty);

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

        public IDictionary RelatedNodeCuePlacementDictionary
        {
            get { return (IDictionary)GetValue(RelatedNodeCuePlacementDictionaryProperty); }
            set { SetValue(RelatedNodeCuePlacementDictionaryProperty, value); }
        }

        public IDictionary MiniButtonPlacementDictionary
        {
            get { return (IDictionary)GetValue(MiniButtonPlacementDictionaryProperty); }
            set { SetValue(MiniButtonPlacementDictionaryProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (AdditionalResourceDictionary != null)
                this.AddResourceDictionary(AdditionalResourceDictionary);

            base.OnApplyTemplate();
        }
    }
}