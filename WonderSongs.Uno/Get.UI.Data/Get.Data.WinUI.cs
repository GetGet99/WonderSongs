using Get.Data.Collections.Update;
using System.Runtime.CompilerServices;
using Get.Data.DataTemplates;
using Get.Data.Collections.Conversion;
using Get.Data.XACL;
using Get.Data.Collections.Implementation;
using Get.Data.Bindings;

namespace Get.Data.Properties
{
    public partial class DependencyPropertyDefinition<TOwnerType, TTargetType>(DependencyProperty dp) : IPropertyDefinition<TOwnerType, TTargetType> where TOwnerType : DependencyObject
    {
        public IProperty<TTargetType> GetProperty(TOwnerType owner)
            => new DPPropertyWrapper<TOwnerType, TTargetType>(owner, dp);

        IReadOnlyProperty<TTargetType> IReadOnlyPropertyDefinition<TOwnerType, TTargetType>.GetProperty(TOwnerType owner)
            => GetProperty(owner);
    }
    public partial class DPPropertyWrapper<TOwnerType, TTargetType> : PropertyBase<TTargetType>, IDisposable where TOwnerType : DependencyObject
    {
        readonly TOwnerType owner;
        readonly DependencyProperty dp;
        readonly long token;
        public DPPropertyWrapper(TOwnerType owner, DependencyProperty dp)
        {
            this.owner = owner;
            this.dp = dp;
            _Value = (TTargetType)owner.GetValue(dp);
            token = owner.RegisterPropertyChangedCallback(dp, delegate
            {
                var oldValue = _Value;
                var newValue = (TTargetType)owner.GetValue(dp);
                ValueChanging?.Invoke(oldValue, newValue);
                _Value = newValue;
                ValueChanged?.Invoke(oldValue, newValue);
            });
        }
        TTargetType _Value;
        public override TTargetType Value { get => _Value; set => owner.SetValue(dp, value); }

        public override event ValueChangingHandler<TTargetType>? ValueChanging;
        public override event ValueChangedHandler<TTargetType>? ValueChanged;
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            try
            {
                owner.UnregisterPropertyChangedCallback(dp, token);
            } catch (ObjectDisposedException) { }
        }
        ~DPPropertyWrapper() => Dispose();
    }
    public static class WinExtension
    {
        public static DependencyPropertyDefinition<TOwnerType, TTargetType> AsPropertyDefinition<TOwnerType, TTargetType>(this DependencyProperty dp) where TOwnerType : DependencyObject
            => new(dp);
        public static DPPropertyWrapper<TOwnerType, TTargetType> AsProperty<TOwnerType, TTargetType>(this DependencyProperty dp, TOwnerType owner) where TOwnerType : DependencyObject
            => new(owner, dp);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TSrc, T>(this UIElementCollection collection, CollectionItemsBinding<TSrc, UIElement> toBind)
        {
            toBind.Source.Bind(collection.AsGDCollection(), toBind.DataTemplate);
        }
    }
}
namespace Get.Data.Collections.Conversion
{
    public static class WinExtension
    {
        public static IGDCollection<UIElement> AsGDCollection(this UIElementCollection c)
        => new UIElementCollectionGDCollection(c);
        readonly struct UIElementCollectionGDCollection(UIElementCollection c) : IGDCollection<UIElement>, IMoveImplGDCollection
        {
            public UIElement this[int index] { get => c[index]; set => c[index] = value; }

            public int Count => c.Count;

            public void Insert(int index, UIElement item)
                => c.Insert(index, item);

            public void RemoveAt(int index)
                => c.RemoveAt(index);
            public void Move(int index1, int index2)
                => c.Move((uint)index1, (uint)index2);
        }
    }
}
namespace Get.Data.XACL
{
    public static class WinExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TSrc>(this UIElementCollection collection, CollectionItemsBinding<TSrc, UIElement> toBind)
        {
            toBind.Source.Bind(collection.AsGDCollection(), toBind.DataTemplate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TSrc>(this UIElementCollection collection, IUpdateReadOnlyCollection<TSrc> source, DataTemplate<TSrc, UIElement> dataTemplate)
        {
            source.Bind(collection.AsGDCollection(), dataTemplate);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(this UIElementCollection collection, IUpdateReadOnlyCollection<UIElement> source)
        {
            source.Bind(collection.AsGDCollection());
        }
    }
}