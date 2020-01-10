using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : AsyncCommandBase
    {
        public ShowDiagramWindowCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
           await HostUiServices.ShowDiagramWindowAsync();
        }
    }
}
