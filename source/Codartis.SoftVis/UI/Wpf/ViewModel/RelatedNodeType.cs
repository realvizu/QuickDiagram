using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A related node type is defined by the directed relationship type that connectes them to a node.
    /// </summary>
    public struct RelatedNodeType
    {
        public DirectedModelRelationshipType RelationshipType { get; }
        public string Name { get; }

        public RelatedNodeType(DirectedModelRelationshipType relationshipType, string name)
        {
            RelationshipType = relationshipType;
            Name = name;
        }
    }
}
