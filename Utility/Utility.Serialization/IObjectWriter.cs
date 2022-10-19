namespace Utility.Serialization
{
    public interface IObjectWriter
    {
        void Write<T>(T instance);
    }
}
