using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Represents a relationship in a Roslyn based model.
    /// </summary>
    internal interface IRoslynRelationship : IModelRelationship
    {
        /// <summary>
        /// Provides an enumerated classification for the model relationship type.
        /// </summary>
        RelationshipStereotype Stereotype { get; }
    }
}
