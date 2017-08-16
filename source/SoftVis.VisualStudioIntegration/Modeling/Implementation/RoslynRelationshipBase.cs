using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.Modeling2.Implementation;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for Roslyn based model relationships.
    /// </summary>
    internal abstract class RoslynRelationshipBase : ModelRelationshipBase, IRoslynRelationship
    {
        public RelationshipStereotype Stereotype { get; }

        protected RoslynRelationshipBase(ModelItemId id, IModelNode source, IModelNode target, RelationshipStereotype stereotype) 
            : base(id, source, target)
        {
            Stereotype = stereotype;
        }
    }
}
