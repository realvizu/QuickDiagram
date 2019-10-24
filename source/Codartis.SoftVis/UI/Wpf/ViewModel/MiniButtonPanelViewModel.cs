using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Collections;
using Codartis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a set of minibuttons that appear on diagram shapes.
    /// Only one shape can have minibuttons at a time.
    /// Tracks focus and controls minibutton visibility and minibutton (decorator) to shape (host) assignment.
    /// </summary>
    /// <remarks>
    /// Caches the minibutton view models created for a certain diagram shape stereotype.
    /// </remarks>
    public class MiniButtonPanelViewModel : DecorationManagerViewModelBase<IDiagramShapeUi>
    {
        private readonly Dictionary<string, List<IMiniButton>> _miniButtonViewModelCache;
        private readonly object _cacheLockObject = new object();

        public ObservableCollection<IMiniButton> ButtonViewModels { get; }

        public MiniButtonPanelViewModel()
        {
            _miniButtonViewModelCache = new Dictionary<string, List<IMiniButton>>();
            ButtonViewModels = new ThreadSafeObservableCollection<IMiniButton>();
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var buttonViewModel in ButtonViewModels)
                buttonViewModel.Dispose();
        }

        protected override IEnumerable<IUiDecorator<IDiagramShapeUi>> GetDecoratorsFor(IDiagramShapeUi hostViewModel)
        {
            lock (_cacheLockObject)
            {
                var hostType = hostViewModel.Stereotype;
                if (_miniButtonViewModelCache.ContainsKey(hostType))
                    return _miniButtonViewModelCache[hostType];

                var miniButtonViewModels = hostViewModel.CreateMiniButtons().ToList();
                _miniButtonViewModelCache.Add(hostType, miniButtonViewModels);

                foreach (var miniButtonViewModel in miniButtonViewModels)
                    ButtonViewModels.Add(miniButtonViewModel);

                return miniButtonViewModels;
            }
        }
    }
}
