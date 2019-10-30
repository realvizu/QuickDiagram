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
//        private readonly IReadOnlyList<IRoslynSymbol> _modelEntities;
//        private readonly bool _followWithViewport;

//        public AddItemsToDiagramCommand(IAppServices appServices, IReadOnlyList<IRoslynSymbol> modelEntities, bool followWithViewport)
//            : base(appServices)
//        {
//            _modelEntities = modelEntities;
//            _followWithViewport = followWithViewport;
//        }

//        public override async Task ExecuteAsync()
//        {
//            var diagramNodes = await ShowProgressAndAddItemsAsync(_modelEntities);

//            await UiService.ShowDiagramWindowAsync();

//            if (_followWithViewport)
//                UiService.FollowDiagramNodes(diagramNodes);
//        }

//        private async Task<IReadOnlyList<IDiagramNode>> ShowProgressAndAddItemsAsync(IReadOnlyList<IRoslynSymbol> modelEntities)
//        {
//            IReadOnlyList<IDiagramNode> diagramNodes = null;

//            using (var progressDialog = await UiService.CreateProgressDialogAsync("Adding model items:", modelEntities.Count))
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

//        private async Task<IReadOnlyList<IDiagramNode>> ShowEntitiesAsync(IReadOnlyList<IRoslynSymbol> modelEntities,
//            CancellationToken cancellationToken, IIncrementalProgress progress)
//        {
//            return await Task.Run(
//                () => DiagramServices.ShowModelNodes(modelEntities, cancellationToken, progress).ToArray(),
//                cancellationToken);
//        }
//    }
//}
