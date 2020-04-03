using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Export
{
    public interface IDiagramExporter
    {
        Task ExportAsync([NotNull] IDiagram diagram, [NotNull] string path);
    }
}
