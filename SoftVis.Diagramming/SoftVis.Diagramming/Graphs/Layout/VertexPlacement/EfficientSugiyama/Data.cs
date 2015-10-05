namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal abstract class Data: IData 
    {
        public int Position { get; set; }

        /* Used by horizontal assignment */
        public readonly Data[] Sinks = new Data[4];
        public readonly double[] Shifts = { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };
    }
}