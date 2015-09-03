using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
    /// <summary>
    /// Interaction logic for DiagramToolWindowControl.
    /// </summary>
    public partial class DiagramToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramToolWindowControl"/> class.
        /// </summary>
        public DiagramToolWindowControl()
        {
            InitializeComponent();

            DiagramViewerControl.DataContext = DiagramBuilder.Instance.Diagram;
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return DiagramViewerControl.GetDiagramAsBitmap();
        }
    }
}