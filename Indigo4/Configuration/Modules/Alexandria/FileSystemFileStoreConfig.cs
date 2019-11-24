using Alexandria;
using Alexandria.FileStores;

namespace Indigo.Configuration.Modules.Alexandria
{
    [Module(typeof(AlexandriaModule))]
    public class FileSystemFileStoreConfig : ConfigBase<FilesystemFileStore>, IFileStoreConfig
    {
        public override FilesystemFileStore InstantiateObject()
        {
            return new FilesystemFileStore();
        }

        Library.IFileStore IFileStoreConfig.InstantiateObject() => InstantiateObject();
    }
}
