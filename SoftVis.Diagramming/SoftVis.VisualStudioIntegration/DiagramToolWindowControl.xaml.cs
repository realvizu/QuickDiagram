using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel;
using System.Windows.Controls;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    /// <summary>
    /// Interaction logic for DiagramToolWindowControl.
    /// </summary>
    public partial class DiagramToolWindowControl : UserControl
    {
        private Diagram _diagram;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramToolWindowControl"/> class.
        /// </summary>
        public DiagramToolWindowControl()
        {
            InitializeComponent();

            var model = TestData.TestModel.Create();
            _diagram = TestData.TestDiagram.Create(model);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DiagramCanvas.DataContext = _diagram;
        }

        public void Add(RoslynBasedUmlModelElement modelElement)
        {
            _diagram.ShowModelElement(modelElement.UmlModelElement);
        }
    }
}