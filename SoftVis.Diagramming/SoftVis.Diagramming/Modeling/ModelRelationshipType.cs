namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides a fixed categorization for model relationships.
    /// Further sub-categories can be introduced with ModelRelationshipStereotype.
    /// </summary>
    public enum ModelRelationshipType
    {
        Generalization,
        Dependency,
        Association,
        Containment
    }
}