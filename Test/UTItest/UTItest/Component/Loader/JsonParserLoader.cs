
namespace UTI
{
    public class JsonParserLoader : AssemblyReader<JsonParserLoader, IJsonParser>
    {
        public override void OnInstanceFinded(IJsonParser instance)
        {
            JsonConverter.SetJsonParser(instance);
        }
    }
}


