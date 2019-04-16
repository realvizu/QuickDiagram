using System;

namespace Codartis.SoftVis.Util
{
    /// <summary>
    /// A progress reporting class that takes count increments and report them further as a cumulated count value.
    /// </summary>
    public class ProgressAccumulator : Progress<int>, IIncrementalProgress
    {
        private int _cumulatedCount;

        public ProgressAccumulator(Action<int> cumulatedValueCallback)
            : base(cumulatedValueCallback)
        {
        }

        public void Reset()
        {
            _cumulatedCount = 0;
        }

        protected override void OnReport(int value)
        {
            _cumulatedCount += value;
            base.OnReport(_cumulatedCount);
        }
    }
}
