namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Base class for commands that can be invoked without a parameter.
    /// </summary>
    internal abstract class ParameterlessCommandBase : CommandBase
    {
        protected ParameterlessCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        public abstract void Execute();
    }
}
