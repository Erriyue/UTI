namespace UTI
{
    public interface IJsonReader
    {
        System.Type DataType { get; }
        object JsontoObject(string json);
    }

}
