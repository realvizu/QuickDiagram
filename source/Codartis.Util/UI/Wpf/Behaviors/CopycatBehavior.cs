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
            new PropertyMetadata(default(DependencyObject)));

        public static readonly DependencyProperty CopiedPropertiesProperty = DependencyProperty.Register(
            "CopiedProperties",
            typeof(DependencyPropertyList),
            typeof(CopycatBehavior));

        public CopycatBehavior()
        {
            SetValue(CopiedPropertiesProperty, new DependencyPropertyList());
        }

        public DependencyObject CopiedObject
        {
            get { return (DependencyObject)GetValue(CopiedObjectProperty); }
            set { SetValue(CopiedObjectProperty, value); }
        }

        public DependencyPropertyList CopiedProperties
        {
            get { return (DependencyPropertyList)GetValue(CopiedPropertiesProperty); }
            set { SetValue(CopiedPropertiesProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            foreach (var copiedProperty in CopiedProperties)
                copiedProperty.Bind(source: CopiedObject, target: AssociatedObject);
        }
    }
}