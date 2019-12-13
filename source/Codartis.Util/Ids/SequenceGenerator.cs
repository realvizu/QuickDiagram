using System.Threading;

namespace Codartis.Util.Ids
{
    public sealed class SequenceGenerator : ISequenceProvider
    {
        private long _lastId;

        public long GetNext()
        {
            return Interlocked.Increment(ref _lastId);
        }
    }
}