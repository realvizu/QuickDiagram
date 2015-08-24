using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using System.Windows.Controls;

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

            DiagramCanvas.DataContext = DiagramBuilder.Instance.Diagram;
        }
    }
}