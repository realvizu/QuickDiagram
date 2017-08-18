using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a bubble list box that attaches to related entity selector diagram shape buttons.
    /// </summary>
    public class RelatedNodeListBoxViewModel : BubbleListBoxViewModel<IModelNode>, IDisposable
    {
        private readonly IDiagram _diagram;

        public RelatedNodeListBoxViewModel(IDiagram diagram)
        {
            _diagram = diagram;
            _diagram.ShapeAdded += OnDiagramShapeAdded;
        }

        public void Dispose()
        {
            _diagram.ShapeAdded -= OnDiagramShapeAdded;
        }

        private RelatedNodeMiniButtonViewModel _ownerButton;

        public RelatedNodeMiniButtonViewModel OwnerButton
        {
            get { return _ownerButton; }
            set
            {
                _ownerButton = value;
                OnPropertyChanged();
            }
        }

        public DiagramShapeViewModelBase OwnerDiagramShape => _ownerButton?.HostViewModel;

        public void Show(RelatedNodeMiniButtonViewModel ownerButton, IEnumerable<IModelNode> items)
        {
            OwnerButton = ownerButton;
            base.Show(items);
        }

        public override void Hide()
        {
            OwnerButton = null;
            base.Hide();
        }

        private void OnDiagramShapeAdded(IDiagramShape diagramShape)
        {
            var itemToRemove = Items.ToArray().FirstOrDefault(i => i.Id == diagramShape?.ModelItem.Id);

            if (itemToRemove != null)
                Items.Remove(itemToRemove);
        }
    }
}
