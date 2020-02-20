using System.Windows;

namespace Codartis.Util.UI.Wpf.Behaviors
{
    /// <summary>
    /// Map a source dependency property to a target.
    /// If target is not specified then it's the same as the source.
    /// </summary>
    public sealed class PropertyMapping
    {
        private DependencyProperty _target;

        public DependencyProperty Source { get; set; }

        public DependencyProperty Target
        {
            get => _target ?? Source;
            set => _target = value;
        }
    }
}