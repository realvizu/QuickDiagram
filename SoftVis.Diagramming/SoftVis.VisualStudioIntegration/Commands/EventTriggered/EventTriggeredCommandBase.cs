using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.EventTriggered
{
    /// <summary>
    /// Base class for all commands that are activated by package events (eg. a diagram node is selected).
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <remarks>
    /// The package automatically discovers all subclasses and routes events to them based on the type parameter.
    /// </remarks>
    internal abstract class EventTriggeredCommandBase<TEventArgs> : CommandBase
        where TEventArgs : EventArgs
    {
        protected EventTriggeredCommandBase(IPackageServices packageServices)
            : base(packageServices)
        {
        }

        public override void Execute(object source, EventArgs args)
        {
            Execute(source, (TEventArgs)args);
        }

        protected abstract void Execute(object source, TEventArgs args);
    }
}