using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;

namespace Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands
{
    /// <summary>
    /// Base class for those commands that behave like a toggle button.
    /// </summary>
    internal abstract class ToggleCommandBase : AsyncCommandBase
    {
        public bool IsChecked { get; private set; }

        protected ToggleCommandBase(IAppServices appServices, bool initialIsChecked = false) 
            : base(appServices)
        {
            IsChecked = initialIsChecked;
        }

        public sealed override async Task ExecuteAsync()
        {
            IsChecked = !IsChecked;

            if (IsChecked)
                await OnCheckedAsync();
            else
                await OnUncheckedAsync();
        }

        protected abstract Task OnCheckedAsync();
        protected abstract Task OnUncheckedAsync();
    }
}
