namespace Utility.ObjectDescription
{
    public interface IObserver
    {
        void OnPropertyChanged(IObservableProperty property, object oldValue);
        void OnRegistered(IObservableProperty property);
        void OnUnregistered(IObservableProperty property);

        void OnRegistered(IObservableObject instance);
        void OnUnregistered(IObservableObject instance);
    }

    public interface IObserver<T> : IObserver
    {
        void OnPropertyChanged(IObservableProperty<T> property, T oldValue);
        void OnRegistered(IObservableProperty<T> property);
        void OnUnregistered(IObservableProperty<T> property);
    }
}