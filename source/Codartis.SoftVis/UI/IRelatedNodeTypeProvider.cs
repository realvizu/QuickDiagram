using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides information about which node types are related to each other by which relationship types.
    /// </summary>
    public interface IRelatedNodeTypeProvider
    {
        IEnumerable<RelatedNodeType> GetRelatedNodeTypes(ModelNodeStereotype stereotype);
    }
}