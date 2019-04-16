namespace Codartis.SoftVis.Modeling.Events
{
    public class ModelClearedEvent : ModelEventBase
    {
        public ModelClearedEvent(IModel newModel) 
            : base(newModel)
        {
        }
    }
}