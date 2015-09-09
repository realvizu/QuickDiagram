namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model relationship is a directed, typed connection between two model entites.
    /// </summary>
    public interface IModelRelationship : IModelItem
    {
        ModelRelationshipType Type { get; }
        IModelEntity Source { get; }
        IModelEntity Target { get; }
    }
}
