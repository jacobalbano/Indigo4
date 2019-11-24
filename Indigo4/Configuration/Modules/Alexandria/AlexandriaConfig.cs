using Alexandria;
using System.Collections.Generic;

namespace Indigo.Configuration.Modules.Alexandria
{
    [Module(typeof(AlexandriaModule))]
    public class AlexandriaConfig : ConfigBase<Library>
    {
        public List<IFileStoreConfig> FileStores { get; set; } = new List<IFileStoreConfig>();
        public List<IFileStoreFactoryConfig> FileStoreFactories { get; set; } = new List<IFileStoreFactoryConfig>();
        public List<ILoaderConfig> Loaders { get; set; } = new List<ILoaderConfig>();

        public override Library InstantiateObject()
        {
            var result = new Library();

            foreach (var fs in FileStores)
                result.AddFileStore(fs.InstantiateObject());

            foreach (var fsf in FileStoreFactories)
                result.AddFileStoreFactory(fsf.InstantiateObject());

            return result;
        }
    }
}
