using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// This content presenter aligns to a chosen UI element, just like an adorner,
    /// and also binds to diagram related visual properties.
    /// </summary>    
    internal class DiagramAdornerlikeContentPresenter : AdornerlikeContentPresenter
    {
        static DiagramAdornerlikeContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramAdornerlikeContentPresenter),
                new FrameworkPropertyMetadata(typeof(DiagramAdornerlikeContentPresenter)));
        }

        public static readonly DependencyProperty DiagramFillProperty =
            DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramAdornerlikeContentPresenter));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramAdornerlikeContentPresenter));

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

        protected override void OnAdornedElementChanged()
        {
            base.OnAdornedElementChanged();

            if (AdornedElement == null)
            {
                this.ClearBinding(DiagramFillProperty);
                this.ClearBinding(DiagramStrokeProperty);
            }
            else
            {
                var diagramVisualProvider = AdornedElement.FindChildren<UIElement>(i => i is IDiagramVisualProvider).FirstOrDefault();
                if (diagramVisualProvider != null)
                {
                    this.SetBinding(DiagramFillProperty, diagramVisualProvider, DiagramVisual.DiagramFillProperty);
                    this.SetBinding(DiagramStrokeProperty, diagramVisualProvider, DiagramVisual.DiagramStrokeProperty);
                }
            }
        }
    }
}