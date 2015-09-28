using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.EfficientSugiyama
{
    internal interface ISegmentContainer : IEnumerable<Segment>, IData, ICloneable 
    {
        /// <summary>
        /// Appends the segment <paramref name="s"/> to the end of the 
        /// container.
        /// </summary>
        /// <param name="s">The segment to append.</param>
        void Append(Segment s);

        /// <summary>
        /// Appends all elements of the container <paramref name="sc"/> to 
        /// this container.
        /// </summary>
        /// <param name="sc"></param>
        void Join(ISegmentContainer sc);

        /// <summary>
        /// Split this container at segment <paramref name="s"/> into two contsiners
        /// <paramref name="sc1"/> and <paramref name="sc2"/>. 
        /// All elements less than s are stored in container <paramref name="sc1"/> and
        /// those who are greated than <paramref name="s"/> in <paramref name="sc2"/>.
        /// Element <paramref name="s"/> is neither in <paramref name="sc1"/> or 
        /// <paramref name="sc2"/>.
        /// </summary>
        /// <param name="s">The segment to split at.</param>
        /// <param name="sc1">The container which contains the elements before <paramref name="s"/>.</param>
        /// <param name="sc2">The container which contains the elements after <paramref name="s"/>.</param>
        void Split(Segment s, out ISegmentContainer sc1, out ISegmentContainer sc2);

        /// <summary>
        /// Split the container at position <paramref name="k"/>. The first <paramref name="k"/>
        /// elements of the container are stored in <paramref name="sc1"/> and the remainder
        /// in <paramref name="sc2"/>.
        /// </summary>
        /// <param name="k">The index where the container should be splitted.</param>
        /// <param name="sc1">The container which contains the elements before <paramref name="s"/>.</param>
        /// <param name="sc2">The container which contains the elements after <paramref name="s"/>.</param>
        void Split(int k, out ISegmentContainer sc1, out ISegmentContainer sc2);

        int Count { get; }
    }
}