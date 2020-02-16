using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI.Wpf.Commands;

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

        public static readonly DependencyProperty FocusRequestedCommandProperty =
            DependencyProperty.Register("FocusRequestedCommand", typeof(DelegateCommand<IDiagramShapeUi>), typeof(DiagramNodeWithCuesControl));

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

        public DelegateCommand<IDiagramShapeUi> FocusRequestedCommand
        {
            get { return (DelegateCommand<IDiagramShapeUi>)GetValue(FocusRequestedCommandProperty); }
            set { SetValue(FocusRequestedCommandProperty, value); }
        }

        public IDictionary RelatedNodeCuePlacementMap
        {
            get { return (IDictionary)GetValue(RelatedNodeCuePlacementMapProperty); }
            set { SetValue(RelatedNodeCuePlacementMapProperty, value); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            FocusRequestedCommand?.Execute(DataContext as DiagramShapeViewModelBase);

            // Must stop the event from bubbling up because if its viewport parent receives MouseMove then it forces the node to lose focus.
            e.Handled = true;
        }
    }
}