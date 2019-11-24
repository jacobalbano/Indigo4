using Indigo.Configuration;
using System;
using System.Text;
using Alexandria;

namespace Indigo.Configuration.Modules.Alexandria
{
    internal class AlexandriaModule : IModule
    {
        string IModule.Name => "Indigo.Alexandria";
    }

    public interface IFileStoreConfig
    {
        Library.IFileStore InstantiateObject();
    }

    public interface IFileStoreFactoryConfig
    {
        Library.IFileStoreFactory InstantiateObject();
    }

    public interface ILoaderConfig
    {
    }
}
