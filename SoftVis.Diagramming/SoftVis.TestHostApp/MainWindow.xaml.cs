using System;
using System.Windows;
using Codartis.SoftVis.TestHostApp.TestData;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var model = TestData.TestModel.Create();
            var diagram = new TestData.TestDiagram(model);
            DiagramViewerControl.DataContext = diagram;

            Dispatcher.BeginInvoke(new Action(() => DiagramViewerControl.FitDiagramToView()));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((TestDiagram)DiagramViewerControl.Diagram).Layout(int.Parse(SweepNumber.Text));
            SweepNumber.Text = (int.Parse(SweepNumber.Text) + 1).ToString();
        }
    }
}
