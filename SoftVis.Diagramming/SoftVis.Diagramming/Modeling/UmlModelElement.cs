namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// The base class of all model elements (packages, types, relationships, etc.)
    /// </summary>
    public abstract class UmlModelElement
    {
        public object NativeItem { get; set; }

        public abstract TResult AcceptVisitor<TResult>(UmlModelVisitorBase<TResult> visitor);
    }
}