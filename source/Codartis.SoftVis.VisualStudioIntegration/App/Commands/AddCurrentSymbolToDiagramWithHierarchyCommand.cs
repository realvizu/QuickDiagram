//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Codartis.SoftVis.Diagramming;
//using Codartis.SoftVis.VisualStudioIntegration.Modeling;
//using Codartis.Util;
//using JetBrains.Annotations;

//namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
//{
//    /// <summary>
//    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
//    /// Shows the diagram if it was not visible.
//    /// </summary>
//    [UsedImplicitly]
//    internal sealed class AddCurrentSymbolToDiagramWithHierarchyCommand : AddCurrentSymbolToDiagramCommandBase
//    {
//        public AddCurrentSymbolToDiagramWithHierarchyCommand(IAppServices appServices)
//            : base(appServices)
//        {
//        }

//        public override async Task ExecuteAsync()
//        {
//            var modelEntity = await ModelService.AddCurrentSymbolAsync();
//            if (modelEntity == null)
//                return;

//            var diagramNodes = await ExtendModelAndDiagramAsync(modelEntity);
//            if (diagramNodes == null)
//                return;

//            await UiService.ShowDiagramWindowAsync();
//            UiService.FollowDiagramNodes(diagramNodes);
//        }

//        private async Task<IReadOnlyList<IDiagramNode>> ExtendModelAndDiagramAsync(IRoslynSymbol symbol)
//        {
//            IReadOnlyList<IDiagramNode> diagramNodes = null;

//            using (var progressDialog = await UiService.CreateProgressDialogAsync("Extending model with entities:"))
//            {
//                await progressDialog.ShowWithDelayAsync();

//                try
//                {
//                    await ExtendModelWithRelatedEntitiesAsync(symbol, progressDialog.CancellationToken, progressDialog.Progress);

//                    progressDialog.Reset("Adding diagram nodes:");

//                    diagramNodes = await ExtendDiagramAsync(symbol, progressDialog.CancellationToken, progressDialog.Progress);
//                }
//                catch (OperationCanceledException)
//                {
//                }
//            }

//            return diagramNodes;
//        }

//        private async Task ExtendModelWithRelatedEntitiesAsync(IRoslynSymbol modelEntity,
//            CancellationToken cancellationToken, IIncrementalProgress progress)
//        {
//            await Task.WhenAll(
//                ModelService.ExtendModelWithRelatedNodesAsync(modelEntity, DirectedModelRelationshipTypes.BaseType, cancellationToken, progress, recursive: true),
//                ModelService.ExtendModelWithRelatedNodesAsync(modelEntity, DirectedModelRelationshipTypes.Subtype, cancellationToken, progress, recursive: true)
//            );
//        }

//        private async Task<IReadOnlyList<IDiagramNode>> ExtendDiagramAsync(IRoslynSymbol symbol,
//            CancellationToken cancellationToken, IIncrementalProgress progress)
//        {
//            return await Task.Run(
//                () => ShowModelNodeWithHierarchy(symbol, cancellationToken, progress).ToArray(),
//                cancellationToken);
//        }

//        public IEnumerable<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynSymbol symbol, 
//            CancellationToken cancellationToken, IIncrementalProgress progress)
//        {
//            var model = ModelService.Model;

//            var baseTypes = model.GetRelatedNodes(symbol.Id, DirectedModelRelationshipTypes.BaseType, recursive: true);
//            var subtypes = model.GetRelatedNodes(symbol.Id, DirectedModelRelationshipTypes.Subtype, recursive: true);
//            var modelNodes = new[] { symbol }.Union(baseTypes).Union(subtypes);

//            return DiagramService.ShowModelNodes(modelNodes, cancellationToken, progress);
//        }
//    }
//}