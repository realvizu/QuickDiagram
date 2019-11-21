using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Defines WPF-specific UI operations.
    /// </summary>
    public interface IWpfDiagramUiService : IDiagramUiService
    {
        /// <summary>
        /// Gets the view of the diagram.
        /// </summary>
        [NotNull]
        DiagramControl DiagramControl { get; }

        /// <summary>
        /// Exports the diagram into a bitmap.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        Task<BitmapSource> CreateDiagramImageAsync(
            double dpi,
            double margin,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            IProgress<int> maxProgress = null);
    }
}