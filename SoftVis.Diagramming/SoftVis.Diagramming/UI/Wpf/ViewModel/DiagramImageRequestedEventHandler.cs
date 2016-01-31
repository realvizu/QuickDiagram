using System;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void DiagramImageRequestedEventHandler(double dpi, Action<BitmapSource> imageCreatedCallback);
}
