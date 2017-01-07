namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Describes the types of components that a diagram connector is made of.
    /// </summary>
    public class ConnectorType
    {
        public ArrowHeadType ArrowHeadType { get; }
        public LineType ShaftLineType { get; }

        public ConnectorType(ArrowHeadType arrowHeadType, LineType shaftLineType)
        {
            ArrowHeadType = arrowHeadType;
            ShaftLineType = shaftLineType;
        }
    }
}
