using System.Collections.Generic;
using System.Linq;
using Codartis.Util.UI.Wpf.Commands;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a set of minibuttons that appear on diagram shapes.
    /// Only one shape can have minibuttons at a time.
    /// If a shape has the minibuttons it is said that it has the focus.
    /// A focus follows the mouse, except when the panel is "pinned".
    /// </summary>
    /// <remarks>
    /// Caches and reuses the created minibutton view models.
    /// </remarks>
    public sealed class MiniButtonPanelViewModel : ViewModelBase, IMiniButtonManager
    {
        /// <summary>
        /// Caches minibutton viewmodels by diagram shape stereotype name.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, List<MiniButtonViewModelBase>> _miniButtonViewModelCache;

        [NotNull] private readonly object _cacheLockObject;

        /// <summary>
        /// The diagram shape that currently has the focus (ie. the minibuttons). Null if none has it.
        /// </summary>
        private IDiagramShapeUi _focusedDiagramShape;

        /// <summary>
        /// The diagram shape that the mouse points at. Null if none.
        /// </summary>
        private IDiagramShapeUi _mouseFocusedDiagramShapeUi;

        /// <summary>
        /// If the focus is pinned then it does not follow the mouse.
        /// </summary>
        private bool _isFocusPinned;

        /// <summary>
        /// The currently visible minibuttons. Null if the panel is not associated with a shape.
        /// </summary>
        private List<MiniButtonViewModelBase> _miniButtonViewModels;

        /// <summary>
        /// Indicates that the mouse points at a different item.
        /// </summary>
        /// <remarks>
        /// If it does not point at a diagram shape then the argument is null.
        /// </remarks>
        public DelegateCommand<IDiagramShapeUi> MouseFocusChangedCommand { get; }

        public MiniButtonPanelViewModel()
        {
            _miniButtonViewModelCache = new Dictionary<string, List<MiniButtonViewModelBase>>();
            _cacheLockObject = new object();

            _focusedDiagramShape = null;
            _isFocusPinned = false;

            MiniButtonViewModels = new List<MiniButtonViewModelBase>();

            MouseFocusChangedCommand = new DelegateCommand<IDiagramShapeUi>(OnMouseFocusChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var buttonViewModels in _miniButtonViewModelCache.Values)
                foreach (var buttonViewModel in buttonViewModels)
                    buttonViewModel.Dispose();
        }

        public IDiagramShapeUi FocusedDiagramShape
        {
            get { return _focusedDiagramShape; }
            set
            {
                _focusedDiagramShape = value;
                OnPropertyChanged();
            }
        }

        public List<MiniButtonViewModelBase> MiniButtonViewModels
        {
            get { return _miniButtonViewModels; }
            set
            {
                _miniButtonViewModels = value;
                OnPropertyChanged();
            }
        }

        private void OnMouseFocusChanged(IDiagramShapeUi diagramShapeUi)
        {
            _mouseFocusedDiagramShapeUi = diagramShapeUi;

            if (_isFocusPinned)
                return;

            if (diagramShapeUi == null)
                UnfocusAll();
            else
                Focus(diagramShapeUi);
        }

        public void Focus(IDiagramShapeUi diagramShapeUi)
        {
            if (_focusedDiagramShape == diagramShapeUi)
                return;

            AssignMiniButtonsTo(diagramShapeUi);
        }

        public void Unfocus(IDiagramShapeUi diagramShapeUi)
        {
            if (_focusedDiagramShape != diagramShapeUi)
                return;

            AssignMiniButtonsTo(null);
        }

        public void UnfocusAll()
        {
            if (_focusedDiagramShape == null)
                return;

            AssignMiniButtonsTo(null);
        }

        public void PinFocus()
        {
            _isFocusPinned = true;
        }

        public void UnpinFocus()
        {
            AssignMiniButtonsTo(_mouseFocusedDiagramShapeUi);
        }

        private void AssignMiniButtonsTo(IDiagramShapeUi diagramShapeUi)
        {
            _isFocusPinned = false;

            if (MiniButtonViewModels != null)
            {
                foreach (var decorator in MiniButtonViewModels)
                    decorator.Hide();
            }

            FocusedDiagramShape = diagramShapeUi;

            if (diagramShapeUi == null)
            {
                MiniButtonViewModels = null;
            }
            else
            {
                MiniButtonViewModels = GetMiniButtonsFor(diagramShapeUi);
                foreach (var miniButton in MiniButtonViewModels)
                    miniButton.AssociateWith(diagramShapeUi);
            }
        }

        private List<MiniButtonViewModelBase> GetMiniButtonsFor([NotNull] IDiagramShapeUi diagramShapeUi)
        {
            lock (_cacheLockObject)
            {
                var hostType = diagramShapeUi.StereotypeName;
                if (_miniButtonViewModelCache.ContainsKey(hostType))
                    return _miniButtonViewModelCache[hostType];

                var miniButtonViewModels = diagramShapeUi.CreateMiniButtons().OfType<MiniButtonViewModelBase>().ToList();
                _miniButtonViewModelCache.Add(hostType, miniButtonViewModels);
                return miniButtonViewModels;
            }
        }
    }
}