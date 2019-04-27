using System;
using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// These are the application services that the application commands can use.
    /// </summary>
    internal interface IAppServices
    {
        IRoslynModelService RoslynModelService { get; }
        IRoslynDiagramService RoslynDiagramService { get; }
        IApplicationUiService ApplicationUiService { get; }

        /// <summary>
        /// Runs an async method from a sync method.
        /// </summary>
        void Run(Func<Task> asyncMethod);
    }
}
