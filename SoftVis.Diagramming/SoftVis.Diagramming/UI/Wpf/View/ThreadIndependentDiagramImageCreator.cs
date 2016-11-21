using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates diagram images. Can be called on other than the main UI thread.
    /// </summary>
    /// <remarks>
    /// Wraps a DiagramImageCreator and ensures that all necessary data (view model and UI elements)
    /// are cloned for execution on a thread that is different from the main UI thread. 
    /// </remarks>
    public class ThreadIndependentDiagramImageCreator
    {
        private readonly DiagramViewModel _diagramViewModel;
        private readonly ResourceDictionary _resourceDictionary;
        private readonly IDiagramStlyeProvider _diagramStlyeProvider;

        public ThreadIndependentDiagramImageCreator(DiagramViewModel diagramViewModel, IDiagramStlyeProvider diagramStlyeProvider,
            ResourceDictionary resourceDictionary = null)
        {
            _diagramViewModel = diagramViewModel;
            _resourceDictionary = resourceDictionary;
            _diagramStlyeProvider = new DiagramStyleCache(diagramStlyeProvider);
        }

        public BitmapSource CreateImage(double dpi, double margin = 0, 
            CancellationToken cancellationToken = default(CancellationToken),
            IProgress<double> progress = null)
        {
            var diagramImageCreator = new DiagramImageCreator(
                Clone(_diagramViewModel.DiagramNodeViewModels),
                Clone(_diagramViewModel.DiagramConnectorViewModelsModels),
                _diagramViewModel.DiagramContentRect,
                _diagramStlyeProvider,
                _resourceDictionary);

            return diagramImageCreator.CreateImage(dpi, margin, cancellationToken, progress);
        }

        private static IEnumerable<DiagramNodeViewModel> Clone(IEnumerable<DiagramNodeViewModel> diagramNodeViewModels)
        {
            return diagramNodeViewModels.Select(i => (DiagramNodeViewModel)i.Clone()).ToArray();
        }

        private static IEnumerable<DiagramConnectorViewModel> Clone(IEnumerable<DiagramConnectorViewModel> diagramConnectorViewModels)
        {
            return diagramConnectorViewModels.Select(i => (DiagramConnectorViewModel)i.Clone()).ToArray();
        }
    }
}
