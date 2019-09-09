using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util;
using Codartis.Util.UI.Wpf.Imaging;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Creates diagram images by setting up a DiagramImageControl and rendering it to a bitmap.
    /// </summary>
    public class DiagramImageCreator : IDiagramImageCreator
    {
        private readonly IEnumerable<DiagramNodeViewModel> _diagramNodeViewModels;
        private readonly IEnumerable<DiagramConnectorViewModel> _diagramConnectorViewModels;
        private readonly Rect _diagramRect;
        private readonly IDiagramStyleProvider _diagramStyleProvider;
        private readonly ResourceDictionary _resourceDictionary;

        public DiagramImageCreator(IEnumerable<DiagramNodeViewModel> diagramNodeViewModels,
            IEnumerable<DiagramConnectorViewModel> diagramConnectorViewModels,
            Rect diagramRect,
            IDiagramStyleProvider diagramStyleProvider,
            ResourceDictionary resourceDictionary = null)
        {
            _diagramNodeViewModels = diagramNodeViewModels;
            _diagramConnectorViewModels = diagramConnectorViewModels;
            _diagramRect = diagramRect;
            _diagramStyleProvider = diagramStyleProvider;
            _resourceDictionary = resourceDictionary;
        }

        public BitmapSource CreateImage(double dpi, double margin = 0, 
            CancellationToken cancellationToken = default, 
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            using (var diagramImageViewModel = new DiagramImageViewModel(_diagramNodeViewModels, _diagramConnectorViewModels, _diagramRect, margin))
            {
                var diagramImageControl = new DiagramImageControl(_resourceDictionary) {DataContext = diagramImageViewModel};
                ApplyVisualProperties(diagramImageControl, _diagramStyleProvider);
                diagramImageControl.EnsureUpToDate();

                var bounds = new Rect(0, 0, diagramImageControl.ActualWidth, diagramImageControl.ActualHeight);
                return UiToBitmapRenderer.RenderUiElementToBitmap(diagramImageControl, bounds, dpi, cancellationToken, progress, maxProgress);
            }
        }

        private static void ApplyVisualProperties(DiagramImageControl diagramImageControl, IDiagramStyleProvider diagramStyleProvider)
        {
            diagramImageControl.Background = diagramStyleProvider.Background;
            diagramImageControl.Foreground = diagramStyleProvider.Foreground;
            diagramImageControl.DiagramFill = diagramStyleProvider.DiagramFill;
            diagramImageControl.DiagramStroke = diagramStyleProvider.DiagramStroke;
            diagramImageControl.FontStyle = diagramStyleProvider.FontStyle;
            diagramImageControl.FontSize = diagramStyleProvider.FontSize;
            diagramImageControl.FontFamily = diagramStyleProvider.FontFamily;
            diagramImageControl.FontStretch = diagramStyleProvider.FontStretch;
            diagramImageControl.FontWeight = diagramStyleProvider.FontWeight;
        }
    }
}
