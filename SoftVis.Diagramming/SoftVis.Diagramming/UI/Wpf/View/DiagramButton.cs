using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// An abstract base for buttons used on diagrams.
    /// Inherits diagram visual properties. 
    /// </summary>
    public abstract class DiagramButton : Button
    {
        static DiagramButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramButton),
                new FrameworkPropertyMetadata(typeof(DiagramButton)));
        }

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramButton));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramButton));

        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register("MouseDoubleClickCommand", typeof(DelegateCommand), typeof(DiagramButton));

        protected DiagramButton()
        {
            IsHitTestVisible = true;
        }

        public Brush DiagramFill
        {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public Brush DiagramStroke
        {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
        }

        public DelegateCommand MouseDoubleClickCommand
        {
            get { return (DelegateCommand)GetValue(MouseDoubleClickCommandProperty); }
            set { SetValue(MouseDoubleClickCommandProperty, value); }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            MouseDoubleClickCommand?.Execute();
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
