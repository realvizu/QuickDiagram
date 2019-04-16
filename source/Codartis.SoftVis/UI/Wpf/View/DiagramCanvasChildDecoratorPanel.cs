using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.Util.UI.Wpf;
using Codartis.Util.UI.Wpf.Controls;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// This panel works like the CanvasChildDecoratorPanel (aligns its children to a chosen canvas child UI element),
    /// and also binds to diagram related visual properties.
    /// </summary>    
    public class DiagramCanvasChildDecoratorPanel : CanvasChildDecoratorPanel
    {
        public static readonly DependencyProperty DiagramFillProperty =
             DiagramVisual.DiagramFillProperty.AddOwner(typeof(DiagramCanvasChildDecoratorPanel));

        public static readonly DependencyProperty DiagramStrokeProperty =
            DiagramVisual.DiagramStrokeProperty.AddOwner(typeof(DiagramCanvasChildDecoratorPanel));

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

        protected override void OnDecoratedElementChanged()
        {
            base.OnDecoratedElementChanged();

            if (DecoratedElement == null)
            {
                this.ClearBinding(DiagramFillProperty);
                this.ClearBinding(DiagramStrokeProperty);
            }
            else
            {
                var diagramVisualProvider = DecoratedElement.FindFirstDescendant<UIElement>(i => i is IDiagramStyleProvider);
                if (diagramVisualProvider != null)
                {
                    this.SetBinding(DiagramFillProperty, diagramVisualProvider, DiagramVisual.DiagramFillProperty);
                    this.SetBinding(DiagramStrokeProperty, diagramVisualProvider, DiagramVisual.DiagramStrokeProperty);
                }
            }
        }
    }
}
