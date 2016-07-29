using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ExplicitlyTriggered
{
    /// <summary>
    /// Activates the source code editor window for a given Symbol.
    /// </summary>
    internal class ShowSourceFileCommand : ExplicitCommandBase<IDiagramNode>
    {
        public ShowSourceFileCommand(IAppServices appServices) 
            : base(appServices)
        {
        }

        public override void Execute(IDiagramNode diagramNode)
        {
            var roslynBasedModelEntity = diagramNode?.ModelEntity as RoslynBasedModelEntity;
            if (roslynBasedModelEntity != null)
                HostWorkspaceServices.ShowSourceFile(roslynBasedModelEntity.RoslynSymbol);

            HostUiServices.DiagramHostWindow.Show();
        }
    }
}
