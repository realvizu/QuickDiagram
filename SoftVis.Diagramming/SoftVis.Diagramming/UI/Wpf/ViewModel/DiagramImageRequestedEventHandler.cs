using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void DiagramImageRequestedEventHandler(Rect bounds, double dpi, Action<BitmapSource> imageCreatedCallback);
}
