namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    public interface ILayoutAlgorithmSelectionStrategy
    {
        IGroupLayoutAlgorithm GetForRoot();
        IGroupLayoutAlgorithm GetForNode(IDiagramNode node);
    }
}