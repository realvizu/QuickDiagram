namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    /// <summary>
    /// Describes the relative location of a vertex: its layer index and its index in the layer.
    /// </summary>
    public struct RelativeLocation
    {
        public int LayerIndex { get; }
        public int IndexInLayer { get; }

        public RelativeLocation(int layerIndex, int indexInLayer)
        {
            LayerIndex = layerIndex;
            IndexInLayer = indexInLayer;
        }

        public bool Equals(RelativeLocation other)
        {
            return LayerIndex == other.LayerIndex && IndexInLayer == other.IndexInLayer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RelativeLocation && Equals((RelativeLocation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LayerIndex*397) ^ IndexInLayer;
            }
        }

        public static bool operator ==(RelativeLocation left, RelativeLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelativeLocation left, RelativeLocation right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({LayerIndex}/{IndexInLayer})";
        }
    }
}
