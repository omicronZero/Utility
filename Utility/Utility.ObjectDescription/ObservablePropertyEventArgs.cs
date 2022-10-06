namespace Utility.ObjectDescription
{
    public class ObservablePropertyEventArgs<T> : System.EventArgs
    {
        private IObservableProperty<T> Property { get; }

        public ObservablePropertyEventArgs(IObservableProperty<T> property)
        {
            Property = property;
        }
    }

    public class ObservablePropertyEventArgs : System.EventArgs
    {
        private IObservableProperty Property { get; }

        public ObservablePropertyEventArgs(IObservableProperty property)
        {
            Property = property;
        }
    }
}