using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Extensibility;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.Common.HitTesting;
using Codartis.SoftVis.Rendering.Wpf.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Modification.MiniButtons;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures.Animated;
using Codartis.SoftVis.Rendering.Wpf.ImageExport;
using Codartis.SoftVis.Rendering.Wpf.InputControls;
using Codartis.SoftVis.Rendering.Wpf.View;

namespace Codartis.SoftVis.Rendering.Wpf
{
    /// <summary>
    /// This control is responsible for displaying the diagram viewport and the PanAndZoomControl,
    /// and hooking together the viewport manipulation events and gestures.
    /// Also responsible for hosting a diagram panel for image export.
    /// </summary>
    [TemplatePart(Name = PART_DiagramViewportPanel, Type = typeof(DiagramViewportPanel))]
    [TemplatePart(Name = PART_PanAndZoomControl, Type = typeof(PanAndZoomControl))]
    [TemplatePart(Name = PART_RelatedEntitySelectorControl, Type = typeof(EntitySelectorControl))]
    public partial class DiagramViewerControl : TemplatedControlBase, IUIEventSource
    {
        private const string PART_DiagramViewportPanel = "PART_DiagramViewportPanel";
        private const string PART_PanAndZoomControl = "PART_PanAndZoomControl";
        private const string PART_RelatedEntitySelectorControl = "PART_RelatedEntitySelectorControl";

        private DiagramViewportPanel _diagramViewportPanel;
        private PanAndZoomControl _panAndZoomControl;
        private EntitySelectorControl _entitySelectorControl;

        private readonly List<IViewportGesture> _gestures = new List<IViewportGesture>();
        private readonly SimpleDiagramPanel _diagramPanelForImageExport = new SimpleDiagramPanel();
        private readonly ResourceDictionary _additionalResourceDictionary;

        static DiagramViewerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramViewerControl),
                new FrameworkPropertyMetadata(typeof(DiagramViewerControl)));
        }

        public DiagramViewerControl()
        {
            DiagramHitTester = new HitTester(this);
        }

        public DiagramViewerControl(ResourceDictionary additionalResourceDictionary) 
            : this()
        {
            _additionalResourceDictionary = additionalResourceDictionary;
        }

        public event PanEventHandler PanWidget;
        public event ZoomEventHandler ZoomWidget;
        public event EventHandler FitToViewWidget;
        public event ResizeEventHandler WindowResized;

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        public IDiagramBehaviourProvider DiagramBehaviourProvider
        {
            get { return (IDiagramBehaviourProvider)GetValue(DiagramBehaviourProviderProperty); }
            set { SetValue(DiagramBehaviourProviderProperty, value); }
        }

        internal IHitTester DiagramHitTester
        {
            get { return (IHitTester)GetValue(DiagramHitTesterProperty); }
            set { SetValue(DiagramHitTesterProperty, value); }
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

        public ICommand ShowRelatedEntitySelectorCommand
        {
            get { return (ICommand)GetValue(ShowRelatedEntitySelectorCommandProperty); }
            set { SetValue(ShowRelatedEntitySelectorCommandProperty, value); }
        }

        public ICommand HideRelatedEntitySelectorCommand
        {
            get { return (ICommand)GetValue(HideRelatedEntitySelectorCommandProperty); }
            set { SetValue(HideRelatedEntitySelectorCommandProperty, value); }
        }

        public void FitDiagramToView()
        {
            EnsureThatDelayedRenderingOperationsAreCompleted();
            OnFitToView();
        }

        public BitmapSource GetDiagramAsBitmap(int dpi)
        {
            EnsureUpToDateDiagramForExport();
            var bitmap = ImageRenderer.RenderUIElementToBitmap(_diagramPanelForImageExport, dpi);
            return bitmap;
        }

        public override void OnApplyTemplate()
        {
            if (_additionalResourceDictionary != null)
                this.AddResourceDictionary(_additionalResourceDictionary);

            base.OnApplyTemplate();

            Focusable = true;
            Focus();

            InitializeDiagramViewportPanel();
            InitializePanAndZoomControl();
            InitializeRelatedEntitySelectorControl();
            InitializeDiagramPanelForImageExport();
            InitializeGestures();
        }

        private void InitializeDiagramViewportPanel()
        {
            _diagramViewportPanel = FindChildControlInTemplate<DiagramViewportPanel>(PART_DiagramViewportPanel);
            _diagramViewportPanel.MiniButtonActivated += OnMiniButtonActivated;

            MouseLeave += _diagramViewportPanel.OnControlMouseLeave;
        }

        private void InitializePanAndZoomControl()
        {
            _panAndZoomControl = FindChildControlInTemplate<PanAndZoomControl>(PART_PanAndZoomControl);
            _panAndZoomControl.Pan += OnPan;
            _panAndZoomControl.FitToView += OnFitToView;
            _panAndZoomControl.Zoom += OnZoom;
            _panAndZoomControl.ZoomValue = _diagramViewportPanel.Zoom;
        }

        private void InitializeRelatedEntitySelectorControl()
        {
            _entitySelectorControl = FindChildControlInTemplate<EntitySelectorControl>(PART_RelatedEntitySelectorControl);
        }

        private void InitializeDiagramPanelForImageExport()
        {
            if (_additionalResourceDictionary != null)
                _diagramPanelForImageExport.AddResourceDictionary(_additionalResourceDictionary);

            _diagramPanelForImageExport.Diagram = Diagram;
            _diagramPanelForImageExport.Background = _diagramViewportPanel.Background;
        }

        private void InitializeGestures()
        {
            _gestures.Add(new AnimatedViewportGesture(new FitToViewViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(500)));
            _gestures.Add(new AnimatedViewportGesture(new ZoomWidgetViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(200)));
            _gestures.Add(new AnimatedViewportGesture(new PanWidgetViewportGesture(_diagramViewportPanel, this), TimeSpan.FromMilliseconds(200)));
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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //HideRelatedEntitySelectorCommand?.Execute(null);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            base.ArrangeOverride(arrangeBounds);

            _diagramViewportPanel?.Arrange(new Rect(arrangeBounds));

            return arrangeBounds;
        }

        private void OnPan(object sender, PanEventArgs args)
        {
            PanWidget?.Invoke(sender, args);
        }

        private void OnFitToView(object sender = null, EventArgs args = null)
        {
            FitToViewWidget?.Invoke(sender ?? this, args ?? EventArgs.Empty);
        }

        private void OnZoom(object sender, ZoomEventArgs args)
        {
            if (_diagramViewportPanel.Zoom.IsEqualWithTolerance(args.NewZoomValue))
                return;

            ZoomWidget?.Invoke(sender, args);
        }

        private void OnResize(Size newSize)
        {
            WindowResized?.Invoke(this, new ResizeEventArgs(newSize.Width, newSize.Height));
        }

        private void OnViewportCommand(object sender, ViewportCommandBase command)
        {
            HideRelatedEntitySelectorCommand?.Execute(null);

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
            return _diagramViewportPanel.Zoom.IsEqualWithTolerance(_panAndZoomControl.ZoomValue);
        }

        private static bool IsCommandAnimatedFromZoomWidgetEvent(ViewportCommandBase command)
        {
            return command.Sender is ZoomWidgetViewportGesture;
        }

        private void EnsureUpToDateDiagramForExport()
        {
            _diagramPanelForImageExport.FontSize = FontSize;
            _diagramPanelForImageExport.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _diagramPanelForImageExport.Arrange(new Rect(_diagramPanelForImageExport.DesiredSize));

            EnsureThatDelayedRenderingOperationsAreCompleted();
        }

        private void EnsureThatDelayedRenderingOperationsAreCompleted()
        {
            Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));
        }

        private void OnMiniButtonActivated(object sender, MiniButtonActivatedEventArgs e)
        {
            ShowRelatedEntitySelectorCommand?.Execute(e);
        }
    }
}
