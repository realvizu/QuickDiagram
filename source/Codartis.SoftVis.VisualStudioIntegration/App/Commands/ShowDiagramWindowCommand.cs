using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    [UsedImplicitly]
    internal sealed class ShowDiagramWindowCommand : CommandBase
    {
        public ShowDiagramWindowCommand([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override Task ExecuteAsync()
        {
            return HostUiService.ShowDiagramWindowAsync();
        }
    }
}