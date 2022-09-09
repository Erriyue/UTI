namespace UTI
{
    public interface IJsonParser
    {
        System.Type DataType { get; }
        string ObjecttoJson(object obj);
    }
}
