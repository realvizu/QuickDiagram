using System;
using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Util.UI.Wpf.Commands;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// A listbox control with a visual handle that must be attached to another control (the owner).
    /// The owner must be specified with its viewmodel and this control will figure out how to attach to it.
    /// The owner viewmodel must be the data context of a control that implements IBubbleListBoxOwner.
    /// The common ancestor of the bubble list box and owner must also be set with the CommonAncestorWithOwner property.
    /// This control must have a Panel ancestor and positions itself relative to that panel.
    /// </summary>
    public class BubbleListBox : ListBox
    {
        static BubbleListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BubbleListBox),
                new FrameworkPropertyMetadata(typeof(BubbleListBox)));
        }

        public static readonly DependencyProperty OwnerViewModelProperty =
            DependencyProperty.Register("OwnerViewModel", typeof(ViewModelBase), typeof(BubbleListBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange,
                    OwnerButtonViewModelProperty_Changed));

        private static void OwnerButtonViewModelProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((BubbleListBox)d).AttachToViewModel((ViewModelBase)e.NewValue);

        public static readonly DependencyProperty CommonAncestorWithOwnerProperty =
            DependencyProperty.Register("CommonAncestorWithOwner", typeof(FrameworkElement), typeof(BubbleListBox));

        public static readonly DependencyProperty HandleOrientationProperty =
            DependencyProperty.Register("HandleOrientation", typeof(HandleOrientation), typeof(BubbleListBox),
                new FrameworkPropertyMetadata(HandleOrientation.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ItemSelectedCommandProperty =
            DependencyProperty.Register("ItemSelectedCommand", typeof(DelegateCommand<object>), typeof(BubbleListBox));

        private IBubbleListBoxOwner _ownerUiElement;
        private Panel _parentPanel;

        public ViewModelBase OwnerViewModel
        {
            get { return (ViewModelBase)GetValue(OwnerViewModelProperty); }
            set { SetValue(OwnerViewModelProperty, value); }
        }

        public FrameworkElement CommonAncestorWithOwner
        {
            get { return (FrameworkElement)GetValue(CommonAncestorWithOwnerProperty); }
            set { SetValue(CommonAncestorWithOwnerProperty, value); }
        }

        public HandleOrientation HandleOrientation
        {
            get { return (HandleOrientation)GetValue(HandleOrientationProperty); }
            set { SetValue(HandleOrientationProperty, value); }
        }

        public DelegateCommand<object> ItemSelectedCommand
        {
            get { return (DelegateCommand<object>)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride() => new BubbleListBoxItem();

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            ReleaseMouseCapture();
            ItemSelectedCommand.Execute(SelectedItem);
            UnselectAll();
        }

        private void AttachToViewModel(ViewModelBase newOwnerViewModel)
        {
            if (_ownerUiElement != null)
                _ownerUiElement.LayoutUpdated -= OnOwnerUiElementLayoutUpdated;

            if (newOwnerViewModel == null)
                return;

            _ownerUiElement = CommonAncestorWithOwner?.FindDescendantByDataContext<IBubbleListBoxOwner>(newOwnerViewModel);
            _parentPanel = this.FindAncestor<Panel>();

            if (_ownerUiElement != null)
                _ownerUiElement.LayoutUpdated += OnOwnerUiElementLayoutUpdated;
        }

        private void OnOwnerUiElementLayoutUpdated(object sender, EventArgs e)
        {
            if (_ownerUiElement == null || _parentPanel == null)
                return;

            HandleOrientation = _ownerUiElement.GetHandleOrientation();

            var ownerAttachPoint = _ownerUiElement.GetAttachPoint();
            var transformFromOwnerToParentPanel = _ownerUiElement.TransformToVisual(_parentPanel);
            var ownerAttachPointInParentCoordinates = transformFromOwnerToParentPanel.Transform(ownerAttachPoint);

            var bubbleAttachPoint = CalculateBubbleAttachPoint();

            VisualOffset = ownerAttachPointInParentCoordinates - bubbleAttachPoint;
        }

        private Point CalculateBubbleAttachPoint()
        {
            var vectorX = ActualWidth / 2;
            var vectorY = HandleOrientation == HandleOrientation.Top ? 0 : ActualHeight;
            return new Point(vectorX, vectorY);
        }
    }
}