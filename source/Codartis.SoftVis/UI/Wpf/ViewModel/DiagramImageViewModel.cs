using System.Collections.Generic;
using System.Windows;
using Codartis.Util.UI.Wpf;
using Codartis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// View model for a control used for generating diagram image.
    /// </summary>
    public class DiagramImageViewModel : ViewModelBase
    {
        public IEnumerable<DiagramNodeViewModel> DiagramNodeViewModels { get; }
        public IEnumerable<DiagramConnectorViewModel> DiagramConnectorViewModels { get; }
        public Size Size { get; }
        public double Margin { get; }
        public Rect ContentRect { get; }

        public DiagramImageViewModel(
            IEnumerable<DiagramNodeViewModel> diagramNodeViewModels,
            IEnumerable<DiagramConnectorViewModel> diagramConnectorViewModels,
            Rect contentRect,
            double margin = 0)
        {
            DiagramNodeViewModels = diagramNodeViewModels;
            DiagramConnectorViewModels = diagramConnectorViewModels;
            Size = contentRect.WithMargin(margin).Size;
            Margin = margin;
            ContentRect = contentRect;
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();
        }
    }
}