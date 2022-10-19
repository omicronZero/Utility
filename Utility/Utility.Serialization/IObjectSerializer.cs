namespace Utility.Serialization
{
    public interface IObjectSerializer
    {
        void GetObjectData<T>(IObjectWriter target, T instance);
        T GetObject<T>(IObjectReader source);
    }
}
