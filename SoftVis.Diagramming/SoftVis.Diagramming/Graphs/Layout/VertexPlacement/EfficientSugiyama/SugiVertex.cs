using System;
using System.Diagnostics;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    [DebuggerDisplay("{Type}: {OriginalVertex} - {Position} ; {MeasuredPosition} on layer {LayerIndex}")]
    internal class SugiVertex : Data
    {
        private int? _layerIndex;
        public ISized OriginalVertex { get; }
        public SugiVertexType Type { get; }
        public Size2D Size { get; }
        public Segment Segment { get; set; }
        public double MeasuredPosition { get; set; }

        public Point2D Center => new Point2D(HorizontalPosition, VerticalPosition);
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

        private SugiVertex(ISized originalVertex, Size2D size)
            : this(SugiVertexType.Original, size)
        {
            OriginalVertex = originalVertex;
        }

        private SugiVertex(SugiVertexType type, Size2D size = new Size2D())
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

        internal static SugiVertex CreateNormal(ISized originalVertex, Size2D size)
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