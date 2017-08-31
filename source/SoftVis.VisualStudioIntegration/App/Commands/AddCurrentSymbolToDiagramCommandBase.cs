namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Abstract base class for those commands that add the current symbol to the diagram.
    /// </summary>
    internal abstract class AddCurrentSymbolToDiagramCommandBase : AsyncCommandBase
    {
        protected AddCurrentSymbolToDiagramCommandBase(IAppServices appServices) 
            : base(appServices)
        {
        }

        public override bool IsEnabled() => ModelService.IsCurrentSymbolAvailableAsync().Result;
    }
}