namespace Utility.Serialization
{
    //Requires constructor with signature .ctr(IObjectReader source)
    public interface IObjectSerializable
    {
        void GetObjectData(IObjectWriter writer);
    }
}
