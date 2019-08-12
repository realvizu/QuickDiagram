namespace Codartis.SoftVis.Modeling.Definition
{
    public abstract class ModelEventBase
    {
        public IModel NewModel { get; }

        protected ModelEventBase(IModel newModel)
        {
            NewModel = newModel;
        }
    }
}