namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
{
    internal struct Span
    {
        public double From { get; }
        public double To { get; }

        public Span(double @from, double to)
        {
            From = @from;
            To = to;
        }

        public double Center => (From + To)/2;

        public override string ToString()
        {
            return $"{From}-{To}";
        }
    }
}
