namespace Codartis.SoftVis.Modeling.Definition.Events
{
    public class ModelClearedEvent : ModelEventBase
    {
        public ModelClearedEvent(IModel newModel) 
            : base(newModel)
        {
        }
    }
}