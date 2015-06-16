namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// The base class of all model elements (packages, types, relationships, etc.)
    /// </summary>
    public abstract class UmlModelElement
    {
        public abstract T AcceptVisitor<T>(UmlModelVisitorBase<T> visitor);
    }
}