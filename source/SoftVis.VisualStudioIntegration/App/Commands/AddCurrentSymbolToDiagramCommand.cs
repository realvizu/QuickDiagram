using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    [UsedImplicitly]
    internal sealed class AddCurrentSymbolToDiagramCommand : AddCurrentSymbolToDiagramCommandBase
    {
        public AddCurrentSymbolToDiagramCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var modelEntity = await ModelService.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            var diagramNode = DiagramServices.ShowModelNode(modelEntity);
            await UiService.ShowDiagramWindowAsync();
            UiService.FollowDiagramNode(diagramNode);
        }
    }
}
