namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for commands that are executed with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the command's parameter.</typeparam>
    internal abstract class ParameterizedCommandBase<T> : CommandBase
    {
        protected ParameterizedCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract void Execute(T param);
    }
}
