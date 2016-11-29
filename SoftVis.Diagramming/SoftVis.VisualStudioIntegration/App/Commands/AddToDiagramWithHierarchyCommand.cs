using System;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramWithHierarchyCommand : AsyncCommandBase
    {
        public AddToDiagramWithHierarchyCommand(IAppServices appServices)
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
            var progressDialog = UiServices.ShowProgressDialog("Extending model with entities:", ProgressMode.Count);
            progressDialog.Show();

            var cancellationToken = progressDialog.CancellationToken;
            var progress = new Progress<int>(i => progressDialog.AddProgressCount(i));

            try
            {
                await Task.Run(() => ExtendModelWithRelatedEntities(modelEntity, cancellationToken, progress), cancellationToken);

                progressDialog.ResetProgressCount();
                progressDialog.SetText("Adding diagram nodes:");

                await Task.Run(() => ExtendDiagram(modelEntity, cancellationToken, progress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                progressDialog.Close();
            }
        }

        private void ExtendModelWithRelatedEntities(IModelEntity modelEntity, CancellationToken cancellationToken, IProgress<int> progress)
        {
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.BaseType, cancellationToken, progress, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.Subtype, cancellationToken, progress, recursive: true);
        }

        private void ExtendDiagram(IRoslynBasedModelEntity modelEntity, CancellationToken cancellationToken, IProgress<int> progress)
        {
            DiagramServices.ShowModelEntityWithHierarchy(modelEntity, cancellationToken, progress);
        }
    }
}
