namespace Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama
{
	public class SugiyamaLayoutParameters : ILayoutParameters
	{
        public LayoutDirection LayoutDirection = LayoutDirection.SourcesAtTop;
	    public double LayerDistance = 30.0;
	    public double VertexDistance = 15.0;
	    public int PositionMode = -1;
	    public bool MinimizeEdgeLength = false;
	    public const int MaxPermutations = 4;
	    public EdgeRoutingType EdgeRoutingType = EdgeRoutingType.Orthogonal;
	}
}