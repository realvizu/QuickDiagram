//------------------------------------------------------------------------------
// <copyright file="DiagramToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Codartis.SoftVis.VisualStudioIntegration
{
    using Diagramming;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

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
    }
}