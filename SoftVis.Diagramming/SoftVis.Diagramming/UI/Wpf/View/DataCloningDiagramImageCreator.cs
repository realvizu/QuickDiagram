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
    /// Creates diagram images by first cloning all necessary data (view models and visual properties) 
    /// so it can be executed other than the main UI thread.
    /// </summary>
    public class DataCloningDiagramImageCreator : IDiagramImageCreator
    {
        private readonly DiagramViewModel _diagramViewModel;
        private readonly ResourceDictionary _resourceDictionary;
        private readonly IDiagramStlyeProvider _diagramStlyeProvider;

        public DataCloningDiagramImageCreator(DiagramViewModel diagramViewModel, IDiagramStlyeProvider diagramStlyeProvider,
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
