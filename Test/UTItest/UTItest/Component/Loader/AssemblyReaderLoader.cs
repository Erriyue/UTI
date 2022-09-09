using System;

namespace UTI
{
    public class AssemblyReaderLoader : AssemblyReader<AssemblyReaderLoader, IAssemblyReader>
    {
        protected static bool Loaded = false;
        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                new AssemblyReaderLoader().FindTypes();
            }
        }

        public override void OnInstanceFinded(IAssemblyReader instance)
        {
            if (instance.GetType() != typeof(AssemblyReaderLoader))
            {
                instance.FindTypes();
            }
        }
    }
}


