namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ExplicitlyTriggered
{
    /// <summary>
    /// Base class for commands that are explicitly triggered from code.
    /// </summary>
    /// <typeparam name="T">The type of the command's parameter.</typeparam>
    internal abstract class ExplicitCommandBase<T> : CommandBase
    {
        protected ExplicitCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract void Execute(T param);
    }
}
