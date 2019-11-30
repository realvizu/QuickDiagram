//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Codartis.SoftVis.Diagramming;
//using Codartis.SoftVis.VisualStudioIntegration.Modeling;
//using Codartis.Util;

//namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
//{
//    /// <summary>
//    /// Adds the given model items to the diagram and shows a progress dialog.
//    /// </summary>
//    internal sealed class AddItemsToDiagramCommand : CommandBase
//    {
//        private readonly IReadOnlyList<IRoslynNode> _modelEntities;
//        private readonly bool _followWithViewport;

//        public AddItemsToDiagramCommand(IAppServices appServices, IReadOnlyList<IRoslynNode> modelEntities, bool followWithViewport)
//            : base(appServices)
//        {
//            _modelEntities = modelEntities;
//            _followWithViewport = followWithViewport;
//        }

//        public override async Task ExecuteAsync()
//        {
//            var diagramNodes = await ShowProgressAndAddItemsAsync(_modelEntities);

//            await HostUiService.ShowDiagramWindowAsync();

//            if (_followWithViewport)
//                HostUiService.FollowDiagramNodes(diagramNodes);
//        }

//        private async Task<IReadOnlyList<IDiagramNode>> ShowProgressAndAddItemsAsync(IReadOnlyList<IRoslynNode> modelEntities)
//        {
//            IReadOnlyList<IDiagramNode> diagramNodes = null;

//            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Adding model items:", modelEntities.Count))
//            {
//                await progressDialog.ShowWithDelayAsync();

//                try
//                {
//                    diagramNodes = await ShowEntitiesAsync(modelEntities, progressDialog.CancellationToken, progressDialog.Progress);
//                }
//                catch (OperationCanceledException)
//                {
//                }
//            }

//            return diagramNodes;
//        }

//        private async Task<IReadOnlyList<IDiagramNode>> ShowEntitiesAsync(IReadOnlyList<IRoslynNode> modelEntities,
//            CancellationToken cancellationToken, IIncrementalProgress progress)
//        {
//            return await Task.Run(
//                () => DiagramService.ShowModelNodes(modelEntities, cancellationToken, progress).ToArray(),
//                cancellationToken);
//        }
//    }
//}
