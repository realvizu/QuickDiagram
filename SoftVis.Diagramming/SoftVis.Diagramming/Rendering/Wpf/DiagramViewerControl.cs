using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Common;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures.Animated;
using Codartis.SoftVis.Rendering.Wpf.ImageExport;
using Codartis.SoftVis.Rendering.Wpf.InputControls;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// Routes user input events to gestures that translate them to viewport commands.
    /// </summary>
    [TemplatePart(Name = PART_DiagramViewportPanel, Type = typeof(DiagramViewportPanel))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    public partial class DiagramViewerControl : Control, IInputElement, IWidgetEventSource
    {
        private const string PART_DiagramViewportPanel = "PART_DiagramViewportPanel";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";

        private DiagramViewportPanel _diagramViewportPanel;
        private PanAndZoomControl _panAndZoomControl;

        private readonly SimpleDiagramPanel _diagramPanelForImageExport = new SimpleDiagramPanel();
        private readonly List<IViewportGesture> _gestures = new List<IViewportGesture>();

        static DiagramViewerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramViewerControl), new FrameworkPropertyMetadata(typeof(DiagramViewerControl)));
        }

        public event PanEventHandler Pan;
        public event ZoomEventHandler Zoom;
        public event EventHandler FitToView;
        public event ResizeEventHandler Resize;

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public void FitDiagramToView()
        {
            EnsureThatDelayedRenderingOperationsAreCompleted();
            OnFitToView();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Focusable = true;
            Focus();

            InitializeDiagramViewportPanel();
            InitializePanAndZoomControl();
            InitializeDiagramPanelForImageExport();
            InitializeGestures();
        }

        private void InitializeDiagramViewportPanel()
        {
            _diagramViewportPanel = GetTemplateChild(PART_DiagramViewportPanel) as DiagramViewportPanel;
        }

        private void InitializePanAndZoomControl()
        {
            _panAndZoomControl = GetTemplateChild(PART_PanAndZoomControl) as PanAndZoomControl;
            _panAndZoomControl.Pan += OnPan;
            _panAndZoomControl.FitToView += OnFitToView;
            _panAndZoomControl.Zoom += OnZoom;
            _panAndZoomControl.ZoomValue = _diagramViewportPanel.Zoom;
        }

        private void InitializeDiagramPanelForImageExport()
        {
            _diagramPanelForImageExport.Diagram = Diagram;
            _diagramPanelForImageExport.Background = _diagramViewportPanel.Background;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedViewportGesture(new FitToViewWidgetEventViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(500)));
            _gestures.Add(new AnimatedViewportGesture(new ZoomWidgetEventViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new PanWidgetEventViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new MouseZoomViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new MousePanViewportGesture(_diagramViewportPanel, this));
            _gestures.Add(new KeyboardViewportGesture(_diagramViewportPanel, this));
            _gestures.Add(new ResizeViewportGesture(_diagramViewportPanel, this));

            foreach (var gesture in _gestures)
            {
                gesture.ViewportCommand += OnViewportCommand;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            OnResize(sizeInfo.NewSize);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            base.ArrangeOverride(arrangeBounds);

            _diagramViewportPanel?.Arrange(new Rect(arrangeBounds));

            return arrangeBounds;
        }

        private void OnPan(object sender, PanEventArgs args)
        {
            Pan?.Invoke(sender, args);
        }

        private void OnFitToView(object sender = null, EventArgs args = null)
        {
            FitToView?.Invoke(sender ?? this, args ?? EventArgs.Empty);
        }

        private void OnZoom(object sender, ZoomEventArgs args)
        {
            if (_diagramViewportPanel.Zoom.EqualsWithTolerance(args.NewZoomValue))
                return;

            Zoom?.Invoke(sender, args);
        }

        private void OnResize(Size newSize)
        {
            Resize?.Invoke(this, new ResizeEventArgs(newSize.Width, newSize.Height));
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            command.Execute(_diagramViewportPanel);
            _diagramViewportPanel.InvalidateArrange();

            if (ViewportZoomMatchesWidgetZoom())
                return;

            if (IsCommandAnimatedFromZoomWidgetEvent(command))
                return;

            _panAndZoomControl.ZoomValue = _diagramViewportPanel.Zoom;
        }

        private bool ViewportZoomMatchesWidgetZoom()
        {
            return _diagramViewportPanel.Zoom.EqualsWithTolerance(_panAndZoomControl.ZoomValue);
        }

        private bool IsCommandAnimatedFromZoomWidgetEvent(ViewportCommandBase command)
        {
            return command.Sender is ZoomWidgetEventViewportGesture;
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            EnsureUpToDateDiagramForExport();
            var bitmap = ImageRenderer.RenderUIElementToBitmap(_diagramPanelForImageExport, 300);
            return bitmap;
        }

        private void EnsureUpToDateDiagramForExport()
        {
            _diagramPanelForImageExport.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _diagramPanelForImageExport.Arrange(new Rect(_diagramPanelForImageExport.DesiredSize));

            EnsureThatDelayedRenderingOperationsAreCompleted();
        }

        private void EnsureThatDelayedRenderingOperationsAreCompleted()
        {
            Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));
        }
    }
}
