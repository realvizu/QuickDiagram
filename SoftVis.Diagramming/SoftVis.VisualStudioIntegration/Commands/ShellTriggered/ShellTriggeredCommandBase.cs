using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Base class for commands that Visual Studio invokes in response to certain shell widgets (toolbar, menu, etc.)
    /// The shell widget that this command responds to is specified in the ctor with its VSCT id.
    /// </summary>
    /// <remarks>
    /// The package automatically discovers all subclasses and registeres them with the Visual Studio Shell.
    /// </remarks>
    internal abstract class ShellTriggeredCommandBase : CommandBase
    {
        /// <summary>
        /// The Guid of a command set that this command belongs to. It is defined in the VSCT (Visual Studio Command Table) file.
        /// </summary>
        public Guid CommandSet { get; }

        /// <summary>
        /// The id of this command. It is defined in the VSCT (Visual Studio Command Table) file.
        /// </summary>
        public int CommandId { get; }

        protected ShellTriggeredCommandBase(Guid commandSet, int commandId, IPackageServices packageServices)
            :base(packageServices)
        {
            CommandSet = commandSet;
            CommandId = commandId;
        }
    }
}
