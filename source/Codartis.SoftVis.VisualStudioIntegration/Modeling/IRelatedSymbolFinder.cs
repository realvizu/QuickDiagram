using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public interface IRelatedSymbolFinder
    {
        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="directedModelRelationshipType">
        /// Optionally specifies what kind of directed relationship types should be navigated to find related symbols.
        /// Null means all.
        /// </param>
        /// <returns>Related Roslyn symbols.</returns>
        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<RelatedSymbolPair>> FindRelatedSymbolsAsync(
            [NotNull] IRoslynModelProvider roslynModelProvider,
            DirectedModelRelationshipType? directedModelRelationshipType = null);
    }
}
