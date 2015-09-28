using System;
using System.Diagnostics;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.EfficientSugiyama
{
    [DebuggerDisplay("{Type}: {OriginalVertex} - {Position} ; {MeasuredPosition} on layer {LayerIndex}")]
    internal class SugiVertex : Data
    {
        private int? _layerIndex;
        public IExtent OriginalVertex { get; }
        public SugiVertexType Type { get; }
        public DiagramSize Size { get; }
        public Segment Segment { get; set; }
        public double MeasuredPosition { get; set; }

        public DiagramPoint Center => new DiagramPoint(HorizontalPosition, VerticalPosition);
        public double Width => Size.Width;
        public double Height => Size.Height;

        public readonly double[] HorizontalPositions = { Double.NaN, Double.NaN, Double.NaN, Double.NaN };
        public double HorizontalPosition = Double.NaN;
        public double VerticalPosition = Double.NaN;

        public readonly SugiVertex[] Roots = new SugiVertex[4];
        public readonly SugiVertex[] Aligns = new SugiVertex[4];
        public readonly double[] BlockWidths = { Double.NaN, Double.NaN, Double.NaN, Double.NaN };
        public int PermutationIndex;
        public bool DoNotOpt;

        private int _tempPosition;

        private SugiVertex(IExtent originalVertex, DiagramSize size)
            : this(SugiVertexType.Original, size)
        {
            OriginalVertex = originalVertex;
        }

        private SugiVertex(SugiVertexType type, DiagramSize size = new DiagramSize())
        {
            Type = type;
            Size = size;
        }

        public int LayerIndex
        {
            get
            {
                if (_layerIndex == null)
                    throw new InvalidOperationException("Layer index is not yet set.");
                return _layerIndex.Value;
            }
            set { _layerIndex = value; }
        }

        public void SavePositionToTemp()
        {
            _tempPosition = Position;
        }

        public void LoadPositionFromTemp()
        {
            Position = _tempPosition;
        }

        internal static SugiVertex CreateNormal(IExtent originalVertex, DiagramSize size)
        {
            return new SugiVertex(originalVertex, size);
        }

        internal static SugiVertex CreatePVertex(Layer targetLayer)
        {
            return CreateDummyVertexOnLayer(SugiVertexType.PVertex, targetLayer);
        }

        internal static SugiVertex CreateQVertex(Layer targetLayer)
        {
            return CreateDummyVertexOnLayer(SugiVertexType.QVertex, targetLayer);
        }

        internal static SugiVertex CreateRVertex(Layer targetLayer)
        {
            return CreateDummyVertexOnLayer(SugiVertexType.RVertex, targetLayer);
        }

        private static SugiVertex CreateDummyVertexOnLayer(SugiVertexType dummyVertexType, Layer targetLayer)
        {
            if (dummyVertexType == SugiVertexType.Original)
                throw new InvalidOperationException("Original vertex should not be created as dummy.");

            var newVertex = new SugiVertex(dummyVertexType);
            targetLayer.AddItem(newVertex);
            return newVertex;
        }
    }
}