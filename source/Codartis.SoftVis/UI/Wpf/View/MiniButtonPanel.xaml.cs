using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Overlays a diagram shape and displays MiniButton controls that manipulate the diagram.
    /// </summary>
    /// <remarks>
    /// In order to position itself over a diagram shape it must copy many properties from certain diagram elements.
    /// Pan and zoom from the diagram canvas, position and size from the diagram shape container
    /// and visual properties (fill and stroke color) from the diagram shape.
    /// </remarks>
    public partial class MiniButtonPanel : UserControl
    {
        public static readonly DependencyProperty PanAndZoomProviderElementProperty =
            DependencyProperty.Register("PanAndZoomProviderElement", typeof(UIElement), typeof(MiniButtonPanel));

        public static readonly DependencyProperty PositionAndSizeProviderElementProperty =
            DependencyProperty.Register("PositionAndSizeProviderElement", typeof(UIElement), typeof(MiniButtonPanel));

        public static readonly DependencyProperty DiagramVisualProviderElementProperty =
            DependencyProperty.Register("DiagramVisualProviderElement", typeof(UIElement), typeof(MiniButtonPanel));

        public static readonly DependencyProperty MiniButtonPlacementMapProperty =
            DependencyProperty.RegisterAttached(
                "MiniButtonPlacementMap",
                typeof(IDictionary),
                typeof(MiniButtonPanel),
                new FrameworkPropertyMetadata(defaultValue: null, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMiniButtonPlacementMap(UIElement element, IDictionary value) => element.SetValue(MiniButtonPlacementMapProperty, value);
        public static IDictionary GetMiniButtonPlacementMap(UIElement element) => (IDictionary)element.GetValue(MiniButtonPlacementMapProperty);

        public MiniButtonPanel()
        {
            InitializeComponent();
        }

        public UIElement PanAndZoomProviderElement
        {
            get { return (UIElement)GetValue(PanAndZoomProviderElementProperty); }
            set { SetValue(PanAndZoomProviderElementProperty, value); }
        }

        public UIElement PositionAndSizeProviderElement
        {
            get { return (UIElement)GetValue(PositionAndSizeProviderElementProperty); }
            set { SetValue(PositionAndSizeProviderElementProperty, value); }
        }

        public UIElement DiagramVisualProviderElement
        {
            get { return (UIElement)GetValue(DiagramVisualProviderElementProperty); }
            set { SetValue(DiagramVisualProviderElementProperty, value); }
        }

        public IDictionary MiniButtonPlacementMap
        {
            get { return (IDictionary)GetValue(MiniButtonPlacementMapProperty); }
            set { SetValue(MiniButtonPlacementMapProperty, value); }
        }
    }
}