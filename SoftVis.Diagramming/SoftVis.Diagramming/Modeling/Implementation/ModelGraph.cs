using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Model item store implemented with QuickGraph.
    /// </summary>
    public class ModelGraph : ConcurrentBidirectionalGraph<IModelEntity, ModelRelationship>
    {
        public ModelGraph() 
            : base(allowParallelEdges: true)
        {
        }
    }
}
