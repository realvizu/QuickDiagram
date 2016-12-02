using System;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddCurrentSymbolToDiagramWithHierarchyCommand : AsyncCommandBase
    {
        public AddCurrentSymbolToDiagramWithHierarchyCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var modelEntity = await ModelServices.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            await ShowProgressAndExtendModel(modelEntity);

            UiServices.ShowDiagramWindow();
            UiServices.FitDiagramToView();
        }

        private async Task ShowProgressAndExtendModel(IRoslynBasedModelEntity modelEntity)
        {
            var progressDialog = UiServices.CreateProgressDialog("Extending model with entities:");
            progressDialog.ShowWithDelayAsync();

            try
            {
                var cancellationToken = progressDialog.CancellationToken;

                await Task.Run(() => ExtendModelWithRelatedEntities(modelEntity, cancellationToken, progressDialog.Progress), cancellationToken);

                progressDialog.Reset("Adding diagram nodes:");

                await Task.Run(() => ExtendDiagram(modelEntity, cancellationToken, progressDialog.Progress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                progressDialog.Close();
            }
        }

        private void ExtendModelWithRelatedEntities(IModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.BaseType, cancellationToken, progress, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.Subtype, cancellationToken, progress, recursive: true);
        }

        private void ExtendDiagram(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            DiagramServices.ShowEntityWithHierarchy(modelEntity, cancellationToken, progress);
        }
    }
}
