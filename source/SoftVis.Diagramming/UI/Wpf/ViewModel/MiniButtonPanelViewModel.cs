using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Codartis.SoftVis.Util.UI.Wpf.Collections;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a set of minibuttons that appear on diagram shapes.
    /// Only one shape can have minibuttons at a time.
    /// Tracks focus and controls minibutton visibility and minibutton (decorator) to shape (host) assignment.
    /// </summary>
    /// <remarks>
    /// Caches the minibutton view models created for a certain kind of diagram shape.
    /// </remarks>
    public class MiniButtonPanelViewModel : DecorationManagerViewModelBase<DiagramShapeViewModelBase>
    {
        private readonly Dictionary<Type, List<MiniButtonViewModelBase>> _miniButtonViewModelCache;
        private readonly object _cacheLockObject = new object();

        public ObservableCollection<MiniButtonViewModelBase> ButtonViewModels { get; }

        public MiniButtonPanelViewModel()
        {
            _miniButtonViewModelCache = new Dictionary<Type, List<MiniButtonViewModelBase>>();
            ButtonViewModels = new ThreadSafeObservableCollection<MiniButtonViewModelBase>();
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var buttonViewModel in ButtonViewModels)
                buttonViewModel.Dispose();
        }

        protected override IEnumerable<IDecoratorViewModel<DiagramShapeViewModelBase>> GetDecoratorsFor(DiagramShapeViewModelBase hostViewModel)
        {
            lock (_cacheLockObject)
            {
                var hostType = hostViewModel.GetType();
                if (_miniButtonViewModelCache.ContainsKey(hostType))
                    return _miniButtonViewModelCache[hostType];

                var miniButtonViewModels = hostViewModel.CreateMiniButtonViewModels().ToList();
                _miniButtonViewModelCache.Add(hostType, miniButtonViewModels);

                foreach (var miniButtonViewModel in miniButtonViewModels)
                    ButtonViewModels.Add(miniButtonViewModel);

                return miniButtonViewModels;
            }
        }
    }
}
