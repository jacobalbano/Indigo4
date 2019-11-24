using Alexandria;
using Alexandria.FileStores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Configuration.Modules.Alexandria
{
    [Module(typeof(AlexandriaModule))]
    public class ZipFileStoreConfig : ConfigBase<ZipFileStore>, IFileStoreConfig
    {
        public string ZipFilePath { get; set; }

        public override ZipFileStore InstantiateObject()
        {
            return new ZipFileStore(ZipFilePath);
        }

        Library.IFileStore IFileStoreConfig.InstantiateObject() => InstantiateObject();
    }

    [Module(typeof(AlexandriaModule))]
    public class ZipFileStoreFactoryConfig : ConfigBase<ZipFileStore.Factory>, IFileStoreFactoryConfig
    {
        public override ZipFileStore.Factory InstantiateObject()
        {
            return new ZipFileStore.Factory();
        }

        Library.IFileStoreFactory IFileStoreFactoryConfig.InstantiateObject() => InstantiateObject();
    }
}
