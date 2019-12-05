using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for all commands.
    /// </summary>
    internal abstract class CommandBase : AppLogicBase
    {
        protected CommandBase([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        [NotNull]
        public virtual Task<bool> IsEnabledAsync() => Task.FromResult(true);

        [NotNull]
        public abstract Task ExecuteAsync();
    }
}