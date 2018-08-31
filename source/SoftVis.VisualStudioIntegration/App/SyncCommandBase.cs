namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Base class for commands that can be invoked synchronously.
    /// </summary>
    internal abstract class SyncCommandBase : CommandBase
    {
        protected SyncCommandBase(IAppServices appServices)
            : base(appServices)
        {

        }
        public virtual bool IsEnabled() => true;
    }
}
