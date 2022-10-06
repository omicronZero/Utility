namespace Utility.ObjectDescription
{
    public class ObservablePropertyChangedEventArgs<T> : ObservablePropertyEventArgs<T>
    {
        public T OldValue { get; }

        public ObservablePropertyChangedEventArgs(IObservableProperty<T> property, T oldValue)
            : base(property)
        {
            OldValue = oldValue;
        }
    }

    public class ObservablePropertyChangedEventArgs : ObservablePropertyEventArgs
    {
        public object OldValue { get; }

        public ObservablePropertyChangedEventArgs(IObservableProperty property, object oldValue)
            : base(property)
        {
            OldValue = oldValue;
        }
    }
}