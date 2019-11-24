using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleAttribute : Attribute
    {
        public readonly Type ModuleType;

        public ModuleAttribute(Type iModule)
        {
            ModuleType = iModule ?? throw new ArgumentNullException(nameof(iModule));
        }
    }

    public interface IModule
    {
        string Name { get; }
    }
}
