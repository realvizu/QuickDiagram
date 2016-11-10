using System.Windows;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A special kind of ContentPresenter for diagram shape items. 
    /// For elements with a DiagramConnectorViewModel datacontext finds their source and target DiagramNodeControls.
    /// </summary>
    public class DiagramShapeItemPresenter : AnimatedRectContentPresenter
    {
        static DiagramShapeItemPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramShapeItemPresenter),
                new FrameworkPropertyMetadata(typeof(DiagramShapeItemPresenter)));
        }

        public static readonly DependencyProperty DiagramConnectorViewModelProperty =
            DependencyProperty.Register("DiagramConnectorViewModel", typeof(DiagramConnectorViewModel), 
                typeof(DiagramShapeItemPresenter), 
                new FrameworkPropertyMetadata(OnDiagramConnectorViewModelChanged));

        private static void OnDiagramConnectorViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((DiagramShapeItemPresenter)d).OnDiagramConnectorViewModelChanged((DiagramConnectorViewModel)e.NewValue);

        public static readonly DependencyProperty SourceRectProviderProperty =
            DependencyProperty.Register("SourceRectProvider", typeof(AnimatedRectContentPresenter), typeof(DiagramShapeItemPresenter));

        public static readonly DependencyProperty TargetRectProviderProperty =
            DependencyProperty.Register("TargetRectProvider", typeof(AnimatedRectContentPresenter), typeof(DiagramShapeItemPresenter));

        public DiagramConnectorViewModel DiagramConnectorViewModel
        {
            get { return (DiagramConnectorViewModel)GetValue(DiagramConnectorViewModelProperty); }
            set { SetValue(DiagramConnectorViewModelProperty, value); }
        }

        public AnimatedRectContentPresenter SourceRectProvider
        {
            get { return (AnimatedRectContentPresenter)GetValue(SourceRectProviderProperty); }
            set { SetValue(SourceRectProviderProperty, value); }
        }

        public AnimatedRectContentPresenter TargetRectProvider
        {
            get { return (AnimatedRectContentPresenter)GetValue(TargetRectProviderProperty); }
            set { SetValue(TargetRectProviderProperty, value); }
        }

        // TODO: move this logic to a more suitable place.
        private void OnDiagramConnectorViewModelChanged(DiagramConnectorViewModel diagramConnectorViewModel)
        {
            SourceRectProvider = this.FindAncestor<DiagramShapeItemsControl>()
                .FindDescendantByDataContext<AnimatedRectContentPresenter>(diagramConnectorViewModel.SourceNode);

            TargetRectProvider = this.FindAncestor<DiagramShapeItemsControl>()
                .FindDescendantByDataContext<AnimatedRectContentPresenter>(diagramConnectorViewModel.TargetNode);
        }
    }
}
