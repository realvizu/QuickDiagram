using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interface for creating diagram images.
    /// </summary>
    public interface IDiagramImageCreator
    {
        /// <summary>
        /// Creates a diagram image.
        /// </summary>
        /// <param name="dpi">The resolution of the image in DPI.</param>
        /// <param name="margin">The size of the margin around the diagram.</param>
        /// <param name="cancellationToken">Token to cancel the image generation, optional.</param>
        /// <param name="progress">Progress riporting object, optional.</param>
        /// <returns>The image of a diagram.</returns>
        BitmapSource CreateImage(double dpi, double margin = 0, 
            CancellationToken cancellationToken = default(CancellationToken), 
            IProgress<double> progress = null);
    }
}