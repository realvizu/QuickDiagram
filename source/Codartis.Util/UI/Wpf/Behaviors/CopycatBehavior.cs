using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.Behaviors
{
    public sealed class CopycatBehavior : Behavior<DependencyObject>
    {
        public static readonly DependencyProperty CopiedObjectProperty = DependencyProperty.Register(
            "CopiedObject",
            typeof(DependencyObject),
            typeof(CopycatBehavior),
            new PropertyMetadata(default(DependencyObject), OnCopiedObjectChanged));

        private static void OnCopiedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CopycatBehavior)d).OnCopiedObjectChanged();

        public static readonly DependencyProperty PropertyMappingsProperty = DependencyProperty.Register(
            "PropertyMappings",
            typeof(PropertyMappingList),
            typeof(CopycatBehavior));

        public CopycatBehavior()
        {
            SetValue(PropertyMappingsProperty, new PropertyMappingList());
        }

        public DependencyObject CopiedObject
        {
            get { return (DependencyObject)GetValue(CopiedObjectProperty); }
            set { SetValue(CopiedObjectProperty, value); }
        }

        public PropertyMappingList PropertyMappings
        {
            get { return (PropertyMappingList)GetValue(PropertyMappingsProperty); }
            set { SetValue(PropertyMappingsProperty, value); }
        }

        private void OnCopiedObjectChanged()
        {
            if (CopiedObject == null)
            {
                foreach (var propertyMapping in PropertyMappings)
                    AssociatedObject.ClearBinding(propertyMapping.Target);
            }
            else
            {
                foreach (var propertyMapping in PropertyMappings)
                    AssociatedObject.SetBinding(propertyMapping.Target, CopiedObject, propertyMapping.Source);
            }
        }
    }
}