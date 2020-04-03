using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition.Export;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Exports the specification of the current diagram to a file.
    /// </summary>
    [UsedImplicitly]
    internal sealed class ExportDiagramSpecToFileCommand : CommandBase
    {
        [NotNull] private readonly IDiagramExporter _diagramExporter;

        public ExportDiagramSpecToFileCommand(
            [NotNull] IAppServices appServices,
            [NotNull] IDiagramExporter diagramExporter)
            : base(appServices)
        {
            _diagramExporter = diagramExporter;
        }

        public override async Task ExecuteAsync()
        {
            var filename = HostUiService.SelectSaveFilename("Save Diagram Spec to File", "JSON file|*.json");
            if (string.IsNullOrWhiteSpace(filename))
                return;

            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Saving diagram spec..", int.MaxValue))
            {
                progressDialog.ShowProgressNumber = false;
                progressDialog.ShowWithDelay();

                await _diagramExporter.ExportAsync(DiagramService.LatestDiagram, filename);
            }
        }
    }
}