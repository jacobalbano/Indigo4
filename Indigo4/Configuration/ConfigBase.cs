using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Indigo.Configuration
{
    public abstract class ConfigBase<T>
    {
        public readonly string ConfiguredObjectId;

        public ConfigBase()
        {
            var attr = GetType().GetCustomAttribute<ModuleAttribute>(true);
            if (attr == null)
                throw new Exception($"Config class '{GetType().Name}' must have a [Module] attribute!");
            
            var module = moduleCache.GetOrAdd(attr.ModuleType, type =>
            {
                var result = Activator.CreateInstance(type) as IModule;
                if (result == null)
                    throw new Exception($"Cannot instantiate '{type.Name}' as IModule");

                if (!moduleNameValidator.IsMatch(result.Name))
                    throw new Exception($"Invalid name '{result.Name}' for module ID");

                return result;
            });

            ConfiguredObjectId = $"{module.Name}.{typeof(T).Name}";
        }

        public virtual T InstantiateObject()
        {
            throw new NotSupportedException($"Type '{typeof(T).Name}' cannot be instantiated in this way");
        }

        public virtual void Validate()
        {
        }

        private ConcurrentDictionary<Type, IModule> moduleCache = new ConcurrentDictionary<Type, IModule>();
        private Regex moduleNameValidator = new Regex(@"[\w.]*[^.]*", RegexOptions.Compiled);
    }
}
