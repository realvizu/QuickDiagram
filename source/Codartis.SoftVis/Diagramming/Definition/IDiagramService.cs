using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Keeps track of the latest diagram instance through mutated instances and publishes change events.
    /// </summary>
    public interface IDiagramService : IDiagramMutator, IDiagramEventSource
    {
        /// <summary>
        /// Adds multiple nodes to the diagram.
        /// Those nodes whose parent are already on the diagram are added to their parents.
        /// </summary>
        /// <remarks>
        /// WARNING: if the child is added before the parent then the parent won't contain the child.
        /// </remarks>
        void AddNodes(
            [NotNull] IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        void AddConnectors(
            [NotNull] IEnumerable<ModelRelationshipId> modelRelationshipIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);
    }
}