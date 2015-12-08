namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Describes the appaerance of a diagram connector.
    /// </summary>
    public class ConnectorStyle
    {
        public ArrowHeadType ArrowHeadType { get; }
        public LineType ShaftLineType { get; }

        public ConnectorStyle(ArrowHeadType arrowHeadType, LineType shaftLineType)
        {
            ArrowHeadType = arrowHeadType;
            ShaftLineType = shaftLineType;
        }
    }
}
