using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Base class for commands that can be invoked asynchronously.
    /// </summary>
    internal abstract class AsyncCommandBase : CommandBase
    {
        protected AsyncCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public virtual Task<bool> IsEnabledAsync() => Task.FromResult(true);
    }
}