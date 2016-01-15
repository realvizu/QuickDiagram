using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Presents a collection of diagram shape view models. Creates a container for each view model 
    /// and that container creates the proper control corresponding to the view model.
    /// </summary>
    /// <remarks>
    /// In order to implement fade out animation when removing an item, the source collection is duplicated.
    /// The original collection is kept and the view model keeps manipulating that, 
    /// but beside that a second collection is created which is the presented collection.
    /// Modifications of the original collection are executed on the presented collection as well,
    /// but in the case of a removal the item gets a chance to perform some action (eg. fade out animation) 
    /// before its actual removal from the presented collection.
    /// </remarks>
    internal class DiagramItemsControl : ItemsControl
    {
        private ObservableCollection<DiagramShapeViewModelBase> _originalItemsSource;
        private ObservableCollection<DiagramShapeViewModelBase> _presentedItemsSource;

        public DiagramItemsControl()
        {
            // To enable mouse hit.
            Background = Brushes.Transparent;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramItemContainer();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramItemContainer;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (ReferenceEquals(newValue, _presentedItemsSource))
                return;

            SetUpDuplicatedItemsSource((ObservableCollection<DiagramShapeViewModelBase>)newValue);
        }

        private void SetUpDuplicatedItemsSource(ObservableCollection<DiagramShapeViewModelBase> viewModels)
        {
            _originalItemsSource = viewModels;
            ((INotifyCollectionChanged)_originalItemsSource).CollectionChanged += OnOriginalCollectionChanged;

            _presentedItemsSource = new ObservableCollection<DiagramShapeViewModelBase>();
            ItemsSource = _presentedItemsSource;
        }

        private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItemsToPresentedCollection(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItemsFromPresentedCollection(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveItemsFromPresentedCollection(_presentedItemsSource);
                    break;
                default:
                    throw new NotImplementedException($"NotifyCollectionChangedAction {e.Action} is not handled.");
            }
        }

        private void AddItemsToPresentedCollection(IList newItems)
        {
            foreach (var newItem in newItems)
                _presentedItemsSource.Add((DiagramShapeViewModelBase)newItem);
        }

        private void RemoveItemsFromPresentedCollection(IList oldItems)
        {
            foreach (var oldItem in oldItems.OfType<DiagramShapeViewModelBase>().ToList())
            {
                var container = ItemContainerGenerator.ContainerFromItem(oldItem) as DiagramItemContainer;
                container?.OnBeforeRemove(OnItemReadyToBeRemoved);
            }
        }

        private void OnItemReadyToBeRemoved(DiagramShapeViewModelBase viewModel)
        {
            _presentedItemsSource.Remove(viewModel);
        }
    }
}
