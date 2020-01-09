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

        public static readonly DependencyProperty RelatedNodeCuePlacementMapProperty =
            DependencyProperty.Register("RelatedNodeCuePlacementMap", typeof(IDictionary), typeof(DiagramControl));

        public static readonly DependencyProperty MiniButtonPlacementMapProperty =
            DependencyProperty.Register("MiniButtonPlacementMap", typeof(IDictionary), typeof(DiagramControl));

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

        public IDictionary RelatedNodeCuePlacementMap
        {
            get { return (IDictionary)GetValue(RelatedNodeCuePlacementMapProperty); }
            set { SetValue(RelatedNodeCuePlacementMapProperty, value); }
        }

        public IDictionary MiniButtonPlacementMap
        {
            get { return (IDictionary)GetValue(MiniButtonPlacementMapProperty); }
            set { SetValue(MiniButtonPlacementMapProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (AdditionalResourceDictionary != null)
                this.AddResourceDictionary(AdditionalResourceDictionary);

            base.OnApplyTemplate();
        }
    }
}