using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Codartis.Util.UI.Wpf.Commands;

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

        public static readonly DependencyProperty MouseFocusedUiElementProperty =
            DependencyProperty.Register(
                "MouseFocusedUiElement",
                typeof(UIElement),
                typeof(MiniButtonPanel),
                new PropertyMetadata(OnMouseFocusedUiElementChanged));

        private static void OnMouseFocusedUiElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((MiniButtonPanel)d).OnMouseFocusedUiElementChanged((UIElement)e.NewValue);

        public static readonly DependencyProperty FocusedDiagramNodeControlProperty = DependencyProperty.Register(
            "FocusedDiagramNodeControl",
            typeof(DiagramNodeControl),
            typeof(MiniButtonPanel));

        public static readonly DependencyProperty MouseFocusedDiagramShapeChangedCommandProperty =
            DependencyProperty.Register(
                "MouseFocusedDiagramShapeChangedCommand",
                typeof(DelegateCommand<IDiagramShapeUi>),
                typeof(MiniButtonPanel));

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

        public UIElement MouseFocusedUiElement
        {
            get { return (UIElement)GetValue(MouseFocusedUiElementProperty); }
            set { SetValue(MouseFocusedUiElementProperty, value); }
        }

        public DiagramNodeControl FocusedDiagramNodeControl
        {
            get { return (DiagramNodeControl)GetValue(FocusedDiagramNodeControlProperty); }
            set { SetValue(FocusedDiagramNodeControlProperty, value); }
        }

        public DelegateCommand<IDiagramShapeUi> MouseFocusedDiagramShapeChangedCommand
        {
            get { return (DelegateCommand<IDiagramShapeUi>)GetValue(MouseFocusedDiagramShapeChangedCommandProperty); }
            set { SetValue(MouseFocusedDiagramShapeChangedCommandProperty, value); }
        }

        private void OnMouseFocusedUiElementChanged(UIElement uiElement)
        {
            var diagramShapeUi = (uiElement as FrameworkElement)?.DataContext as IDiagramShapeUi;

            MouseFocusedDiagramShapeChangedCommand?.Execute(diagramShapeUi);
        }
    }
}