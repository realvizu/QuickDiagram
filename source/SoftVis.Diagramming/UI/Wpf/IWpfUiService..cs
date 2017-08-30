using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Defines WPF-specific UI operations.
    /// </summary>
    public interface IWpfUiService : IUiService
    {
        DiagramViewModel DiagramViewModel { get; }

        void Initialize(ResourceDictionary resourceDictionary, IDiagramStlyeProvider diagramStlyeProvider);

        Task<BitmapSource> CreateDiagramImageAsync(double dpi, double margin,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
