using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    internal abstract class CommandBase
    {
        public Guid CommandSet { get; }
        public int CommandId { get; }
        protected IHostServices HostServices { get; }

        protected CommandBase(Guid commandSet, int commandId, IHostServices hostServices)
        {
            CommandSet = commandSet;
            CommandId = commandId;
            HostServices = hostServices;
        }

        public abstract void Execute(object sender, EventArgs e);
    }
}
