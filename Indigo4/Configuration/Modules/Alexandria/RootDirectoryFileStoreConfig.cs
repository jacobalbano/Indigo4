using Alexandria;
using Alexandria.FileStores;

namespace Indigo.Configuration.Modules.Alexandria
{
    [Module(typeof(AlexandriaModule))]
    public class RootDirectoryFileStoreConfig : ConfigBase<RootDirectoryFileStore>, IFileStoreConfig
    {
        public string RootDirectoryPath { get; set; }

        public override RootDirectoryFileStore InstantiateObject()
        {
            return new RootDirectoryFileStore(RootDirectoryPath);
        }

        Library.IFileStore IFileStoreConfig.InstantiateObject() => InstantiateObject();
    }
}
