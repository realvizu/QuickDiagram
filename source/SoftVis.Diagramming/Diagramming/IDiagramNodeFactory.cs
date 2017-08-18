using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Responsible for creating diagram node objects.
    /// </summary>
    public interface IDiagramNodeFactory
    {
        IDiagramNode CreateDiagramNode(IModelNode modelNode);
    }
}