namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Defines model-related operations.
    /// </summary>
    public interface IModelService : IModelStore
    {
        void AddNode(IModelNode node, IModelNode parentNode);
        bool TryGetParentNode(ModelNodeId modelNodeId, out IModelNode parentNode);
    }
}
