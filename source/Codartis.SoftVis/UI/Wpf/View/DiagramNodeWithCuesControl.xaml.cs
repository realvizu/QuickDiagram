using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for DiagramNodeWithCuesControl.xaml
    /// </summary>
    public partial class DiagramNodeWithCuesControl : UserControl
    {
        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramNodeWithCuesControl));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramNodeWithCuesControl));

        public static readonly DependencyProperty RelatedNodeCuePlacementMapProperty =
            DependencyProperty.RegisterAttached(
                "RelatedNodeCuePlacementMap",
                typeof(IDictionary),
                typeof(DiagramNodeWithCuesControl),
                new FrameworkPropertyMetadata(defaultValue: null, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetRelatedNodeCuePlacementMap(UIElement element, IDictionary value) => element.SetValue(RelatedNodeCuePlacementMapProperty, value);
        public static IDictionary GetRelatedNodeCuePlacementMap(UIElement element) => (IDictionary)element.GetValue(RelatedNodeCuePlacementMapProperty);

        public DiagramNodeWithCuesControl()
        {
            InitializeComponent();
            Focusable = true;
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

        public IDictionary RelatedNodeCuePlacementMap
        {
            get { return (IDictionary)GetValue(RelatedNodeCuePlacementMapProperty); }
            set { SetValue(RelatedNodeCuePlacementMapProperty, value); }
        }
    }
}