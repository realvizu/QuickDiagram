using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Imaging;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates diagram images by setting up a DiagramImageControl and rendering it to a bitmap.
    /// </summary>
    public class DiagramImageCreator : IDiagramImageCreator
    {
        private readonly IEnumerable<DiagramNodeViewModelBase> _diagramNodeViewModels;
        private readonly IEnumerable<DiagramConnectorViewModel> _diagramConnectorViewModels;
        private readonly Rect _diagramRect;
        private readonly IDiagramStlyeProvider _diagramStlyeProvider;
        private readonly ResourceDictionary _resourceDictionary;

        public DiagramImageCreator(IEnumerable<DiagramNodeViewModelBase> diagramNodeViewModels,
            IEnumerable<DiagramConnectorViewModel> diagramConnectorViewModels,
            Rect diagramRect,
            IDiagramStlyeProvider diagramStlyeProvider,
            ResourceDictionary resourceDictionary = null)
        {
            _diagramNodeViewModels = diagramNodeViewModels;
            _diagramConnectorViewModels = diagramConnectorViewModels;
            _diagramRect = diagramRect;
            _diagramStlyeProvider = diagramStlyeProvider;
            _resourceDictionary = resourceDictionary;
        }

        public BitmapSource CreateImage(double dpi, double margin = 0, 
            CancellationToken cancellationToken = default(CancellationToken), 
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            using (var diagramImageViewModel = new DiagramImageViewModel(_diagramNodeViewModels, _diagramConnectorViewModels, _diagramRect, margin))
            {
                var diagramImageControl = new DiagramImageControl(_resourceDictionary) {DataContext = diagramImageViewModel};
                ApplyVisualProperties(diagramImageControl, _diagramStlyeProvider);
                diagramImageControl.EnsureUpToDate();

                var bounds = new Rect(0, 0, diagramImageControl.ActualWidth, diagramImageControl.ActualHeight);
                return UiToBitmapRenderer.RenderUiElementToBitmap(diagramImageControl, bounds, dpi, cancellationToken, progress, maxProgress);
            }
        }

        private static void ApplyVisualProperties(DiagramImageControl diagramImageControl, IDiagramStlyeProvider diagramStlyeProvider)
        {
            diagramImageControl.Background = diagramStlyeProvider.Background;
            diagramImageControl.Foreground = diagramStlyeProvider.Foreground;
            diagramImageControl.DiagramFill = diagramStlyeProvider.DiagramFill;
            diagramImageControl.DiagramStroke = diagramStlyeProvider.DiagramStroke;
            diagramImageControl.FontStyle = diagramStlyeProvider.FontStyle;
            diagramImageControl.FontSize = diagramStlyeProvider.FontSize;
            diagramImageControl.FontFamily = diagramStlyeProvider.FontFamily;
            diagramImageControl.FontStretch = diagramStlyeProvider.FontStretch;
            diagramImageControl.FontWeight = diagramStlyeProvider.FontWeight;
        }
    }
}
