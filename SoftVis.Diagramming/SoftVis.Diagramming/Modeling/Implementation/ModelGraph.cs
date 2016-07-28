using QuickGraph;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Model item store implemented with QuickGraph.
    /// </summary>
    public class ModelGraph : BidirectionalGraph<IModelEntity, ModelRelationship>
    {
        public ModelGraph() 
            : base(allowParallelEdges: true)
        {
        }
    }
}
