namespace Utility.Serialization
{
    public interface IObjectReader
    {
        T Read<T>();
    }
}
