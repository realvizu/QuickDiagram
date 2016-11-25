namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for commands that can be invoked without a parameter.
    /// </summary>
    internal abstract class SyncCommandBase : CommandBase
    {
        protected SyncCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract void Execute();
    }
}
