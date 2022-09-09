using System;

namespace UTI
{
    public interface IAssemblyReader
    {
        void FindTypes();
        void OnTypesFinded(Type type);
        void OnInstanceFinded(object instance);

    }
}


