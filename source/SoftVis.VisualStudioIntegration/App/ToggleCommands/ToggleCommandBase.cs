namespace Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands
{
    /// <summary>
    /// Base class for those commands that behave like a toggle button.
    /// </summary>
    internal abstract class ToggleCommandBase : CommandBase
    {
        public bool IsChecked { get; private set; }

        protected ToggleCommandBase(IAppServices appServices, bool initialIsChecked = false) 
            : base(appServices)
        {
            IsChecked = initialIsChecked;
        }

        public void Execute()
        {
            IsChecked = !IsChecked;

            if (IsChecked)
                OnChecked();
            else
                OnUnchecked();
        }

        protected abstract void OnChecked();
        protected abstract void OnUnchecked();
    }
}
