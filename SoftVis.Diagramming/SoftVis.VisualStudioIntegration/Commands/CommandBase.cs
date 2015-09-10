using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// A command is on operation that Visual Studio invokes in response to certain user interactions (toolbar, menu, etc.)
    /// This is the abstract base class for commands.
    /// </summary>
    internal abstract class CommandBase
    {
        /// <summary>
        /// The Guid of a command set that this command belongs to. It is defined in the VSCT (Visual Studio Command Table) file.
        /// </summary>
        public Guid CommandSet { get; }

        /// <summary>
        /// The id of this command. It is defined in the VSCT (Visual Studio Command Table) file.
        /// </summary>
        public int CommandId { get; }

        /// <summary>
        /// The command can consume package services via this interface.
        /// </summary>
        protected IPackageServices PackageServices { get; }

        protected CommandBase(Guid commandSet, int commandId, IPackageServices packageServices)
        {
            CommandSet = commandSet;
            CommandId = commandId;
            PackageServices = packageServices;
        }

        /// <summary>
        /// This is the logic that will be executed when the command is invoked.
        /// The PackageServices interface can be used to invoke functionality from the hosting VS package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Execute(object sender, EventArgs e);
    }
}
