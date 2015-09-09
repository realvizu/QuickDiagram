using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A model entity is model item that can have relationships to other model entites.
    /// Eg. a class.
    /// </summary>
    public interface IModelEntity : IModelItem
    {
        string Name { get; }
        ModelEntityType Type { get; }
        IEnumerable<IModelRelationship> OutgoingRelationships { get; }
    }
}
