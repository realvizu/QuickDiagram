using System.Collections.Generic;
using System.Windows;
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
        public Rect Rect { get; }

        public DiagramImageViewModel(IEnumerable<DiagramNodeViewModel> diagramNodeViewModels,
            IEnumerable<DiagramConnectorViewModel> diagramConnectorViewModels, 
            Rect contentRect, double margin = 0)
        {
            DiagramNodeViewModels = diagramNodeViewModels;
            DiagramConnectorViewModels = diagramConnectorViewModels;
            Rect = CalculateExportImageRect(contentRect, margin);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var diagramNodeViewModel in DiagramNodeViewModels)
                diagramNodeViewModel.Dispose();

            foreach (var diagramConnectorViewModel in DiagramConnectorViewModels)
                diagramConnectorViewModel.Dispose();
        }

        private static Rect CalculateExportImageRect(Rect diagramContentRect, double margin)
        {
            var diagramContentSize = diagramContentRect.Size;
            var size = new Size(diagramContentSize.Width + 2 * margin, diagramContentSize.Height + 2 * margin);
            var location = new Point(diagramContentRect.Left - margin, diagramContentRect.Top - margin);
            return new Rect(location, size);
        }
    }
}
