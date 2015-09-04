using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    /// <summary>
    /// Manages controls created for diagram shapes.
    /// </summary>
    public abstract partial class DiagramPanelBase : Panel
    {
        private readonly Dictionary<DiagramShape, DiagramShapeControlBase> _diagramShapeControls =
            new Dictionary<DiagramShape, DiagramShapeControlBase>();

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        public Rect DiagramRect
        {
            get { return (Rect)GetValue(DiagramRectProperty); }
            set { SetValue(DiagramRectProperty, value); }
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramPanel = (DiagramPanelBase)obj;
            var diagram = (Diagram)e.NewValue;
            diagramPanel.AddDiagram(diagram);

            diagram.ShapeAdded += diagramPanel.OnShapeAdded;
            diagram.ShapeModified += diagramPanel.OnShapeModified;
            diagram.ShapeRemoved += diagramPanel.OnShapeRemoved;
            diagram.Cleared += diagramPanel.OnDiagramCleared;
        }

        protected virtual void OnDiagramChanged()
        {
            DiagramRect = Diagram.GetEnclosingRect().ToWpf();
        }

        private void AddDiagram(Diagram diagram)
        {
            AddAllGraphElements(diagram);
            OnDiagramChanged();
        }

        private void OnShapeAdded(object sender, DiagramShape shape)
        {
            if (_diagramShapeControls.ContainsKey(shape))
                return;

            if (shape is DiagramNode)
                CreateDiagramNodeControl((DiagramNode)shape);
            else if (shape is DiagramConnector)
                CreateDiagramConnectorControl((DiagramConnector)shape);

            OnDiagramChanged();
        }

        private void OnShapeModified(object sender, DiagramShape shape)
        {
            if (!_diagramShapeControls.ContainsKey(shape))
                return;

            _diagramShapeControls[shape].Update();

            OnDiagramChanged();
        }

        private void OnShapeRemoved(object sender, DiagramShape shape)
        {
            if (!_diagramShapeControls.ContainsKey(shape))
                return;

            RemoveDiagramShapeControl(shape);

            OnDiagramChanged();
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            Children.Clear();
            _diagramShapeControls.Clear();

            OnDiagramChanged();
        }

        private void AddAllGraphElements(Diagram diagram)
        {
            foreach (var node in diagram.Nodes)
                CreateDiagramNodeControl(node);

            foreach (var connector in diagram.Connectors)
                CreateDiagramConnectorControl(connector);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = DiagramNodeControlFactory.CreateFrom(diagramNode);
            _diagramShapeControls.Add(diagramNode, control);
            Children.Add(control);
        }

        private void CreateDiagramConnectorControl(DiagramConnector diagramConnector)
        {
            var control = DiagramConnectorControlFactory.CreateFrom(diagramConnector, _diagramShapeControls);
            _diagramShapeControls.Add(diagramConnector, control);
            Children.Add(control);
        }

        private void RemoveDiagramShapeControl(DiagramShape diagramShape)
        {
            Children.Remove(_diagramShapeControls[diagramShape]);
            _diagramShapeControls.Remove(diagramShape);
        }
    }
}
