namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Makes the diagram window visible.
    /// </summary>
    internal sealed class ShowDiagramWindowCommand : ParameterlessCommandBase
    {
        public ShowDiagramWindowCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiServices.ShowDiagramWindow();
        }
    }
}
