using System;
using Codartis.SoftVis.VisualStudioIntegration.App;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Describes all information needed for registering a command with the host.
    /// </summary>
    /// <typeparam name="T">The type that implements the command.</typeparam>
    internal struct CommandSpecification<T> : ICommandSpecification
        where T : CommandBase
    {
        public int CommandId { get; }

        public CommandSpecification(int commandId)
        {
            CommandId = commandId;
        }

        public Type CommandType => typeof(T);
    }
}
