using Codartis.SoftVis.VisualStudioIntegration.Events;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.EventTriggered
{
    /// <summary>
    /// Activates the source code editor window for a given Symbol.
    /// </summary>
    internal class ShowSourceFileCommand : EventTriggeredCommandBase<DiagramNodeActivatedEventArgs>
    {
        public ShowSourceFileCommand(IPackageServices packageServices) 
            : base(packageServices)
        {
        }

        protected override void Execute(object source, DiagramNodeActivatedEventArgs args)
        {
            var roslynBasedClass = args?.DiagramNode?.ModelEntity as RoslynBasedModelEntity;
            if (roslynBasedClass != null)
            {
                var workspaceServices = PackageServices.GetWorkspaceServices();
                workspaceServices.ShowSourceFile(roslynBasedClass.RoslynSymbol);
            }
        }
    }
}
