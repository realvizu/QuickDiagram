using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Base class for all commands. Commands are the application logic of the package.
    /// </summary>
    internal abstract class CommandBase
    {
        /// <summary>
        /// The command can consume package services via this interface.
        /// </summary>
        protected IPackageServices PackageServices { get; }

        protected CommandBase(IPackageServices packageServices)
        {
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