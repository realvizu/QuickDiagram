using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// A viewport host is a control that hosts a viewport and some content. 
    /// </summary>
    internal interface IViewportHost : IInputElement, IAnimatable
    {
        double MinZoom { get; }
        double MaxZoom { get; }
        double ViewportZoom { get; }
        Rect ViewportInScreenSpace { get; }
        Rect ViewportInDiagramSpace { get; }
        Rect ContentInDiagramSpace { get; }
        Cursor Cursor { get; set; }
    }
}
