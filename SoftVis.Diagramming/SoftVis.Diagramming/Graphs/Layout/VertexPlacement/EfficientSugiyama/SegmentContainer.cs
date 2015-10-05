using System.Collections.Generic;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal class SegmentContainer : List<Segment>, ISegmentContainer
    {
        public SegmentContainer() { }

        private SegmentContainer(int capacity)
            : base(capacity) { }

        public void Append(Segment s)
        {
            Add(s);
        }

        public void Join(ISegmentContainer sc)
        {
            AddRange(sc);
        }

        public void Split(Segment s, out ISegmentContainer sc1, out ISegmentContainer sc2)
        {
            int index = IndexOf(s);
            Split(index, out sc1, out sc2, false);
        }

        public void Split(int k, out ISegmentContainer sc1, out ISegmentContainer sc2)
        {
            Split(k, out sc1, out sc2, true);
        }

        private void Split(int k, out ISegmentContainer sc1, out ISegmentContainer sc2, bool keep)
        {
            int sc1Count = k + (keep ? 1 : 0);
            int sc2Count = Count - k - 1;

            sc1 = new SegmentContainer(sc1Count);
            sc2 = new SegmentContainer(sc2Count);

            for (int i = 0; i < sc1Count; i++)
                sc1.Append(this[i]);

            for (int i = k + 1; i < Count; i++)
                sc2.Append(this[i]);
        }

        public int Position { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}