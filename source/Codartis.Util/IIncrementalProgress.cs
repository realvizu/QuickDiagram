using System;

namespace Codartis.Util
{
    /// <summary>
    /// A provider for progress updates where the updates are increments of a counter.
    /// </summary>
    public interface IIncrementalProgress : IProgress<int>
    {
    }
}
