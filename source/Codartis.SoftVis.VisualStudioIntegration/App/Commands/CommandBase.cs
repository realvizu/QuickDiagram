using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for all commands.
    /// </summary>
    internal abstract class CommandBase : AppLogicBase
    {
        protected CommandBase(IAppServices appServices) : base(appServices)
        {
        }

        public virtual Task<bool> IsEnabledAsync() => Task.FromResult(true);

        public abstract Task ExecuteAsync();
    }
}