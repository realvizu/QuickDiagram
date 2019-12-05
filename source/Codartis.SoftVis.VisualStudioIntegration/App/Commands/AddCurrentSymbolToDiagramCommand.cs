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
        public AddCurrentSymbolToDiagramCommand([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var maybeModelNode = await TryAddCurrentSymbolToDiagramAsync();
            if (!maybeModelNode.HasValue)
                return;

            await HostUiService.ShowDiagramWindowAsync();
            DiagramWindowService.FollowDiagramNode(maybeModelNode.Value.Id);
        }
    }
}