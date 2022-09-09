
namespace UTI
{
    public class JsonReaderLoader : AssemblyReader<JsonReaderLoader, IJsonReader>
    {
        public override void OnInstanceFinded(IJsonReader instance)
        {
            JsonReader.SetJsonReader(instance);
        }
    }


}


