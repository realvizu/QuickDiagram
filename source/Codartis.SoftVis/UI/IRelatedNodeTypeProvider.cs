using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Provides information about which node types are related to each other by which relationship types.
    /// </summary>
    public interface IRelatedNodeTypeProvider
    {
        [NotNull]
        IEnumerable<RelatedNodeType> GetRelatedNodeTypes(ModelNodeStereotype stereotype);
    }
}