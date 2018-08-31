using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for commands that can be invoked asynchronously with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of command parameter.</typeparam>
    internal abstract class AsyncCommandWithParameterBase<T> : AsyncCommandBase
    {
        protected AsyncCommandWithParameterBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract Task ExecuteAsync(T param);
    }

    internal abstract class AsyncCommandWithParameterBase<T1,T2> : AsyncCommandBase
    {
        protected AsyncCommandWithParameterBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract Task ExecuteAsync(T1 param1, T2 param2);
    }
}
