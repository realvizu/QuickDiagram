using System.Windows;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Describes the state of the viewport.
    /// </summary>
    public class ViewportDescriptor
    {
        public Size SizeInScreenSpace { get; }
        public Point CenterInDiagramSpace { get; }
        public double LinearZoom { get; }
        public double MinZoom { get; }
        public double MaxZoom { get; }
        public double ExponentialZoom { get; }
        public TransitionSpeed TransitionSpeed { get; }

        public ViewportDescriptor(Size sizeInScreenSpace, Point centerInDiagramSpace,
            double linearZoom, double minZoom, double maxZoom, double exponentialZoom,
            TransitionSpeed transitionSpeed)
        {
            SizeInScreenSpace = sizeInScreenSpace;
            CenterInDiagramSpace = centerInDiagramSpace;
            LinearZoom = linearZoom;
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            ExponentialZoom = exponentialZoom;
            TransitionSpeed = transitionSpeed;
        }
    }
}
