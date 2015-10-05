namespace Codartis.SoftVis.Common
{
    /// <summary>
    /// A span of two double values.
    /// </summary>
    internal struct DoubleSpan
    {
        public double From { get; }
        public double To { get; }

        public DoubleSpan(double @from, double to)
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
