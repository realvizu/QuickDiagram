using System;

namespace Codartis.SoftVis.Util
{
    /// <summary>
    /// A provider for progress updates where the updates are increments of a counter.
    /// </summary>
    public interface IIncrementalProgress : IProgress<int>
    {
    }
}
