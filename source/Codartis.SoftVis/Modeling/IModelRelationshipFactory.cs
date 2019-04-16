namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Creates model relationships.
    /// </summary>
    public interface IModelRelationshipFactory
    {
        IModelRelationship CreateRelationship(IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype);
    }
}
