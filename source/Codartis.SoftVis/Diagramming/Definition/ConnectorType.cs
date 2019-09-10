namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Describes the types of components that a diagram connector is made of.
    /// </summary>
    public struct ConnectorType
    {
        public ArrowHeadType ArrowHeadType { get; }
        public LineType ShaftLineType { get; }

        public ConnectorType(ArrowHeadType arrowHeadType, LineType shaftLineType)
        {
            ArrowHeadType = arrowHeadType;
            ShaftLineType = shaftLineType;
        }

        public override string ToString() => $"Head:{ArrowHeadType}|Shaft:{ShaftLineType}";
    }
}
