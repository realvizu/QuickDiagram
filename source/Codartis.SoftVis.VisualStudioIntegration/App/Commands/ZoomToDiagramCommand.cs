using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Positions the viewport so that the whole diagram is visible.
    /// </summary>
    [UsedImplicitly]
    internal sealed class ZoomToDiagramCommand : CommandBase
    {
        public ZoomToDiagramCommand([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await HostUiService.ShowDiagramWindowAsync();
            DiagramWindowService.ZoomToDiagram();
        }
    }
}