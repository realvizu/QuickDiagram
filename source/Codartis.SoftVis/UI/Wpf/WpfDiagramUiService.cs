using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Implements a WPF UI service. 
    ///  </summary>
    public class WpfDiagramUiService : IWpfDiagramUiService
    {
        [NotNull] public DiagramViewModel DiagramViewModel { get; }
        public DiagramControl DiagramControl { get; }
        [NotNull] private readonly Func<DiagramViewModel, IDiagramStyleProvider, IDiagramImageCreator> _diagramImageCreatorFactory;

        public WpfDiagramUiService(
            [NotNull] IDiagramUi diagramUi,
            [NotNull] DiagramControl diagramControl,
            [NotNull] Func<DiagramViewModel, IDiagramStyleProvider, IDiagramImageCreator> diagramImageCreatorFactory)
        {
            DiagramViewModel = (DiagramViewModel)diagramUi;

            DiagramControl = diagramControl;
            DiagramControl.DataContext = DiagramViewModel;

            _diagramImageCreatorFactory = diagramImageCreatorFactory;
        }

        public IDiagramUi DiagramUi => DiagramViewModel;

        public async Task<BitmapSource> CreateDiagramImageAsync(
            double dpi,
            double margin,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            IProgress<int> maxProgress = null)
        {
            try
            {
                // The image creator must be created on the UI thread so it can read the necessary view and view model data.
                var diagramImageCreator = _diagramImageCreatorFactory.Invoke(DiagramViewModel, DiagramControl);

                return await Task.Factory.StartSTA(
                    () =>
                        diagramImageCreator.CreateImage(dpi, margin, cancellationToken, progress, maxProgress),
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public void ZoomToDiagram() => DiagramViewModel.ZoomToContent();
        public void FollowDiagramNode(ModelNodeId nodeId) => DiagramViewModel.FollowDiagramNodes(new[] { nodeId });
        public void FollowDiagramNodes(IReadOnlyCollection<ModelNodeId> nodeIds) => DiagramViewModel.FollowDiagramNodes(nodeIds);
        public void KeepDiagramCentered() => DiagramViewModel.KeepDiagramCentered();

        public event ShowModelItemsEventHandler ShowModelItemsRequested
        {
            add => DiagramViewModel.ShowModelItemsRequested += value;
            remove => DiagramViewModel.ShowModelItemsRequested -= value;
        }

        public event Action<IDiagramNode, Size2D> DiagramNodeHeaderSizeChanged
        {
            add => DiagramViewModel.DiagramNodeHeaderSizeChanged += value;
            remove => DiagramViewModel.DiagramNodeHeaderSizeChanged -= value;
        }

        public event Action<IDiagramNode, Point2D> DiagramNodeChildrenAreaTopLeftChanged
        {
            add => DiagramViewModel.DiagramNodeChildrenAreaTopLeftChanged += value;
            remove => DiagramViewModel.DiagramNodeChildrenAreaTopLeftChanged -= value;
        }

        public event Action<IDiagramNode> DiagramNodeInvoked
        {
            add => DiagramViewModel.DiagramNodeInvoked += value;
            remove => DiagramViewModel.DiagramNodeInvoked -= value;
        }

        public event Action<IDiagramNode> RemoveDiagramNodeRequested
        {
            add => DiagramViewModel.RemoveDiagramNodeRequested += value;
            remove => DiagramViewModel.RemoveDiagramNodeRequested -= value;
        }
    }
}