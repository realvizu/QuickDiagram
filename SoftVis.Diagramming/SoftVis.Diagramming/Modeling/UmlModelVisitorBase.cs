namespace Codartis.SoftVis.Modeling
{
    public abstract class UmlModelVisitorBase<TResult>
    {
        public TResult Visit(UmlModelElement item)
        {
            return item.AcceptVisitor(this);
        }

        public virtual TResult Visit(UmlModel item) { return default(TResult); }
        public virtual TResult Visit(UmlPackage item) { return default(TResult); }
        public virtual TResult Visit(UmlClass item) { return default(TResult); }
        public virtual TResult Visit(UmlInterface item) { return default(TResult); }
        public virtual TResult Visit(UmlGeneralization item) { return default(TResult); }
        public virtual TResult Visit(UmlDependency item) { return default(TResult); }
    }
}
