namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Responsible for modifying a model: adding, updating and removing model items.
    /// The underlying model is immutable so each modification creates a new snapshot of the model.
    /// </summary>
    public interface IModelBuilder : INotifyModelChanged
    {
        IModel CurrentModel { get; }

        //IModel AddNode(IModelNode node, IModelNode parentNode = null);
        //IModel RemoveNode(IModelNode node);
        //IModel UpdateNode(IModelNode node, IModelNode newNode);

        //IModel AddRelationship(IModelRelationship relationship);
        //IModel RemoveRelationship(IModelRelationship relationship);
        IModelBuilder ClearModel();
    }
}
