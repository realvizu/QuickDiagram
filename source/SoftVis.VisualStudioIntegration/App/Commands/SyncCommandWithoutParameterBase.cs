namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for commands that can be invoked synchronously without a parameter.
    /// </summary>
    internal abstract class SyncCommandWithoutParameterBase : SyncCommandBase
    {
        protected SyncCommandWithoutParameterBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract void Execute();
    }
}
