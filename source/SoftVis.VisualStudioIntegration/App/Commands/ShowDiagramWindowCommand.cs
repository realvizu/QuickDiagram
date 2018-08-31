namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : SyncCommandWithoutParameterBase
    {
        public ShowDiagramWindowCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiService.ShowDiagramWindow();
        }
    }
}
