namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Defines model-related operations.
    /// </summary>
    public interface IModelService : IModelMutator
    {
        void AddNode(IModelNode node, IModelNode parentNode);
        bool TryGetParentNode(ModelNodeId modelNodeId, out IModelNode parentNode);
    }
}
