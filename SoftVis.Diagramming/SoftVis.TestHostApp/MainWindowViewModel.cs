using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codartis.SoftVis.TestHostApp.TestData;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.Util.UI.Wpf.Controls;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.TestHostApp
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly TestModel _testModel;
        private readonly TestDiagram _testDiagram;

        public MainWindow Window { get; set; }

        private int _modelItemGroupIndex;
        private int _nextToRemoveModelItemGroupIndex;
        private double _selectedDpi;

        public DiagramViewModel DiagramViewModel { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ZoomToContentCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public IDiagramStlyeProvider DiagramStlyeProvider { get; set; }

        private ProgressWindowViewModel _progressViewModel;
        private ProgressWindow _progressWindow;
        private CancellationTokenSource _imageExportCancellationTokenSource;

        public MainWindowViewModel()
        {
            _testModel = new TestModelBuilder().Create();
            //_testModel = new BigTestModelBuilder().Create(4, 4);

            _testDiagram = new TestDiagram(_testModel);
            _testDiagram.ShapeActivated += shape => Debug.WriteLine($"Activated: {shape.ModelItem.ToString()}");

            DiagramViewModel = new DiagramViewModel(_testDiagram, minZoom: 0.2, maxZoom: 5, initialZoom: 1);

            AddCommand = new DelegateCommand(AddShapes);
            RemoveCommand = new DelegateCommand(RemoveShapes);
            ZoomToContentCommand = new DelegateCommand(ZoomToContent);
            CopyToClipboardCommand = new DelegateCommand(CopyToClipboard);

            SelectedDpi = 300;
        }

        public double SelectedDpi
        {
            get { return _selectedDpi; }
            set
            {
                _selectedDpi = value;
                OnPropertyChanged();
            }
        }

        private void AddShapes()
        {
            if (_modelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            _testDiagram.ShowItems(_testDiagram.ModelItemGroups[_modelItemGroupIndex]);
            _modelItemGroupIndex++;

            //_testDiagram.Save(@"c:\big.xml");

            ZoomToContent();
        }

        private void RemoveShapes()
        {
            if (_nextToRemoveModelItemGroupIndex == _testDiagram.ModelItemGroups.Count)
                return;

            _testDiagram.HideItems(_testDiagram.ModelItemGroups[_nextToRemoveModelItemGroupIndex]);
            _nextToRemoveModelItemGroupIndex++;

            ZoomToContent();
        }

        private void ZoomToContent()
        {
            var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            timer.Tick += (s, o) =>
            {
                timer.Stop();
                DiagramViewModel.ZoomToContent();
            };
            timer.Start();
        }

        private void CopyToClipboard()
        {
            ShowProgressWindow();

            CreateDiagramImageAsync()
                .ContinueInCurrentContext(SetImageToClipboard)
                .ContinueInCurrentContext(i => CloseProgressWindow());
        }

        private async Task<BitmapSource> CreateDiagramImageAsync()
        {
            try
            {
                var diagramImageCreator = new DataCloningDiagramImageCreator(DiagramViewModel, DiagramStlyeProvider);
                return await Task.Factory.StartSTA(() =>
                {
                    var progress = new Progress<double>(SetProgress);
                    var cancellationToken = _imageExportCancellationTokenSource.Token;
                    return diagramImageCreator.CreateImage(SelectedDpi, 10, cancellationToken, progress);
                });
            }
            catch (OutOfMemoryException)
            {
                HandleOutOfMemory();
                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception in CreateDiagramImageAsync: {e}");
                throw;
            }
        }

        private static void SetImageToClipboard(Task<BitmapSource> task)
        {
            try
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Clipboard.SetImage(task.Result);
            }
            catch (OutOfMemoryException)
            {
                HandleOutOfMemory();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception in SetImageToClipboard: {e}");
                throw;
            }
        }

        private static void HandleOutOfMemory()
        {
            MessageBox.Show("Cannot export the image because it is too large. Please select a smaller DPI value.", "TestHostApp");
        }

        private void ShowProgressWindow()
        {
            _imageExportCancellationTokenSource = new CancellationTokenSource();

            _progressViewModel = new ProgressWindowViewModel
            {
                Title = "TestHostApp",
                Text = "Generating image..",
            };
            _progressWindow = new ProgressWindow
            {
                DataContext = _progressViewModel,
                Owner = Window
            };
            _progressWindow.Closed += ProgressWindowOnClosed;

            ShowNonBlockingModal(_progressWindow);
        }

        private void CloseProgressWindow()
        {
            CloseNonBlockingModal(_progressWindow);
        }

        private void ProgressWindowOnClosed(object sender, EventArgs eventArgs)
        {
            _progressWindow.Closed -= ProgressWindowOnClosed;
            _imageExportCancellationTokenSource.Cancel();
            _imageExportCancellationTokenSource.Dispose();
        }

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        private static void EnableWindow(Window window, bool enable)
        {
            var handle = new WindowInteropHelper(window).Handle;
            EnableWindow(handle, enable);
        }

        private static void ShowNonBlockingModal(Window window)
        {
            EnableWindow(window.Owner, false);

            window.Closing += SpecialDialogWindow_Closing;
            window.Show();
        }

        private static void SpecialDialogWindow_Closing(object sender, CancelEventArgs e)
        {
            var window = (Window)sender;
            window.Closing -= SpecialDialogWindow_Closing;

            var owner = window.Owner;
            EnableWindow(owner, true);
            owner.Activate();
        }

        private static void CloseNonBlockingModal(Window window)
        {
            window.Close();
        }

        private void SetProgress(double progress)
        {
            _progressViewModel.ProgressValue = progress;
        }
    }
}
